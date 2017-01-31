using UnityEngine;
using System.Collections;

public class DayCycle : MonoBehaviour {
	public Light sun_l;
	public Color day_color;
	public Color sunrise_color;
	public Color sunset_color;
	public Color night_ambient_color;
	public Color day_ambient_color;
	public float time_coefficient=1; //time speed
	public float daytime=0.25f;

	const int DAY_LENGTH=1440;//minutes
	const float SUNSET_START=0.65f;
	const float SUNSET_END=0.78f;
	const float SUNRISE_START=0.24f;
	const float SUNRISE_END=0.35f;
	const float SUN_NORMAL_INTENS=1;

	const float SKY_NIGHT_INTENS=0.5f;
	const float SKY_DAWN_INTENS=0.6f;
	const float SKY_DAY_INTENS=0.8f;
	const float SKY_DUSK_INTENS=0.6f;
	const float SKY_NIGHT_REF_INTENS=0.6f; //reflective intencity
	const float SKY_DAWN_REF_INTENS=0.7f;
	const float SKY_DAY_REF_INTENS=1f;
	const float SKY_DUSK_REF_INTENS=0.8f;

	Transform sun_t;
	float r_speed=1; //sun rotation speed
	float t_speed;
	byte daypart=255;


	void Awake () 
	{
		if (sun_l) sun_t=sun_l.transform;
		t_speed=1.0f/DAY_LENGTH*time_coefficient;
		r_speed=360.0f/DAY_LENGTH*time_coefficient;

		if (daytime<SUNRISE_START||daytime>SUNSET_END) {daypart=0;}
		else 
		{
			if (daytime<SUNRISE_END) daypart=1;
			else {
				if (daytime>SUNSET_START) daypart=3;
				else daypart=2;
			}
		}
		SetDaypart(daypart);
		float angle=daytime*360.0f;
		sun_t.rotation=Quaternion.Euler(Vector3.left*90);
		sun_t.Rotate(Vector3.right*angle,Space.Self);
	}

	void Update() 
	{
		if (!sun_l) return;
		sun_t.Rotate(Vector3.right*r_speed*Time.deltaTime,Space.Self);
		if (daytime>1) 
		{
			daytime-=((int)daytime);
		}
		daytime+=t_speed*Time.deltaTime;

		float p=0; //percentage
		switch (daypart) {
		case 0: //NIGHT
			if (daytime>=SUNRISE_START&&daytime<SUNSET_END) SetDaypart(1);
		break;
		case 1://DAWN
			p=(daytime-SUNRISE_START)/(SUNRISE_END-SUNRISE_START);
			sun_l.intensity=p*SUN_NORMAL_INTENS;
			sun_l.color=Color.Lerp(sunrise_color,day_color,p);
			if (p<0.5f) 
			{  //from night to dawn
				RenderSettings.ambientSkyColor=Color.Lerp(night_ambient_color,sunrise_color,p*2);
				RenderSettings.ambientIntensity=Mathf.Lerp(SKY_NIGHT_INTENS,SKY_DAWN_INTENS,p*2);
				RenderSettings.reflectionIntensity=Mathf.Lerp(SKY_NIGHT_REF_INTENS,SKY_DAWN_REF_INTENS,p*2);
			}
			else 
			{
				RenderSettings.ambientSkyColor=Color.Lerp(sunrise_color,day_ambient_color,p*2);
				RenderSettings.ambientIntensity=Mathf.Lerp(SKY_DAWN_INTENS,SKY_DAY_INTENS,p*2);
				RenderSettings.reflectionIntensity=Mathf.Lerp(SKY_DAWN_REF_INTENS,SKY_DAY_REF_INTENS,p*2);
			}

			if (daytime>SUNRISE_END) SetDaypart(2);
			break;
		case 2: //DAY
			if (daytime>SUNSET_START) SetDaypart(3);
			break;
		case 3: //SUNSET
			p=(daytime-SUNSET_START)/(SUNSET_END-SUNSET_START);
			sun_l.intensity=(1-p)*SUN_NORMAL_INTENS;
			sun_l.color=Color.Lerp(day_color,sunset_color,p);
			if (p>0.5f) {//from dusk to night
				RenderSettings.ambientSkyColor=Color.Lerp(sunrise_color,night_ambient_color,p*2);
				RenderSettings.ambientIntensity=Mathf.Lerp(SKY_DUSK_INTENS,SKY_NIGHT_INTENS,p*2);
				RenderSettings.reflectionIntensity=Mathf.Lerp(SKY_DUSK_REF_INTENS,SKY_NIGHT_REF_INTENS,p*2);
			}
			else 
			{
				RenderSettings.ambientSkyColor=Color.Lerp(day_ambient_color,sunrise_color,p*2);
				RenderSettings.ambientIntensity=Mathf.Lerp(SKY_DAY_INTENS,SKY_DUSK_INTENS,p*2);
				RenderSettings.reflectionIntensity=Mathf.Lerp(SKY_DAY_REF_INTENS,SKY_DUSK_REF_INTENS,p*2);
			}

			if (daytime>SUNSET_END) SetDaypart(0);
			break;
		}
	}
		
	void SetDaypart(byte x) 
	{
		switch (x) 
		{
		case 0: //NIGHT
			sun_l.intensity=0;
			RenderSettings.ambientIntensity=SKY_NIGHT_INTENS;
			RenderSettings.ambientSkyColor=night_ambient_color;
			RenderSettings.reflectionIntensity=SKY_NIGHT_REF_INTENS;
			daypart=0;
			print ("night");
			break;
		case 1: //DAWN
			sun_l.intensity=0;
			daypart=1;
			print ("ich commes der sonne");
			RenderSettings.ambientIntensity=SKY_DAWN_INTENS;
			RenderSettings.ambientSkyColor=sunrise_color;
			RenderSettings.reflectionIntensity=SKY_DAWN_REF_INTENS;
			break;
		case 2: //DAY
			sun_l.intensity=1;
			sun_l.color=day_color;
			daypart=2;
			RenderSettings.ambientIntensity=SKY_DAY_INTENS	;
			RenderSettings.ambientSkyColor=day_ambient_color;
			RenderSettings.reflectionIntensity=SKY_DAY_REF_INTENS;
			print ("day comes");
			break;
		case 3: // SUNSET
			sun_l.intensity=1;
			sun_l.color=day_color;
			daypart=3;
			RenderSettings.ambientIntensity=SKY_DUSK_INTENS;
			RenderSettings.ambientSkyColor=sunset_color;
			RenderSettings.reflectionIntensity=SKY_DUSK_REF_INTENS;
			print ("the sun falls down");
			break;
		}
	}

	void OnGUI () 
	{
		GUILayout.Label(daytime.ToString());
	}
}
