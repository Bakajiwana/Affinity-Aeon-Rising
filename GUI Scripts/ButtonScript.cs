using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour 
{
	public void LevelLoad(int _index)
	{
		Application.LoadLevel (_index);
	}

	public void LevelLoad(string _index)
	{
		Application.LoadLevel (_index);
	}
}
