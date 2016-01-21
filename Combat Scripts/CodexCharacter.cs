using UnityEngine;
using System.Collections;

//This struct will store the variables of the character's codex. It is a struct because it is more optimised. 

public struct CodexCharacter
{
	public Transform portrait; 
	public string name; 
	public int level;
	public int health;
	public int healthMax;
	public int shield;
	public int shieldMax;
	public int shieldAffinity;
	public int healthAffinity;
	public bool[] statusBuff;
	public bool[] statusDebuff;
	public bool[] statusImmunity;
	public int attack;
	public int defence;
	public int agility; 
	public int luck;
	public int accuracy; 
	public int speed; 

	public bool shieldReveal; 
	public bool healthReveal; 
	public bool noteRevealed; 
	public Transform notes;

	public bool isEnemy;
	public int index;

	public CodexCharacter(Transform _portrait, string _name, int _level, int _shieldAffinity, 
	                      int _currentShield, int _shieldMax, bool _shieldReveal, 
	                      int _healthAffinity, int _currentHealth, int _healthMax,
	                      bool _healthReveal, int[] _statusBuffLength, int[] _statusDebuffLength, 
	                      bool[] _statusImmunity, int _attack, int _defence, int _agility, int _luck, int _accuracy,
	                      int _speed, bool _noteRevealed, Transform _notes, bool _isEnemy, int _index)
	{
		portrait = _portrait; 
		name = _name; 
		level = _level;
		shieldAffinity = _shieldAffinity;
		healthAffinity = _healthAffinity;

		health = _currentHealth;
		shield = _currentShield;

		healthMax = _healthMax;
		shieldMax = _shieldMax;

		statusBuff = new bool[4];
		statusDebuff = new bool[10];

		for(int i = 0; i < _statusBuffLength.Length; i++)
		{
			if(_statusBuffLength[i] > 0)
			{
				statusBuff[i] = true;
			}
			else
			{
				statusBuff[i] = false;
			}
		}

		for(int i = 0; i < _statusDebuffLength.Length; i++)
		{
			if(_statusDebuffLength[i] > 0)
			{
				statusDebuff[i] = true;
			}
			else
			{
				statusDebuff[i] = false;
			}
		}

		statusImmunity = _statusImmunity;

		attack = _attack;
		defence = _defence;
		agility = _agility; 
		luck = _luck;
		accuracy = _accuracy; 
		speed = _speed; 
		
		shieldReveal = _shieldReveal; 
		healthReveal = _healthReveal; 
		noteRevealed = _noteRevealed; 

		notes = _notes;

		isEnemy = _isEnemy;
		index = _index;
	}
}
