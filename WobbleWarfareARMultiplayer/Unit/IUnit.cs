using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Infantry,
    Cavalry,
    Archer,
    Catapult
}

public interface IUnit
{
    UnitType UnitType { get; }

    bool KillInfantry { get;}
    bool KillCavalry { get; }
    bool KillArcher { get; }
    bool KillCatapult { get; }

    int MaxHealth { get; }
    int CurrentHealth { get; set; }
    int Damage { get; }
    int Speed { get; }
    bool Selected { get; set; }

    void Move();
    void Attack(IUnit target);
    void TakeDamage(int damage);
    void ToggleSelected(bool selectedState);
}
