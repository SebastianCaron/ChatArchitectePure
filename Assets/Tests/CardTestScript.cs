using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CardTestScript
{

    private LandCardManager _land;
    
    [SetUp]
    public void InitLand()
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<LandCardManager>();
        _land = gameObject.GetComponent<LandCardManager>();
    }

    [Test]
    public void CanTakeDamage()
    {
        CardData building = ScriptableObject.CreateInstance<CardData>();
        building.behaviour = CardBehaviourEnum.PRODUCTION_GOLD;
        building.type = CardTypeEnum.BUILDING;
        building.life = 10f;
        building.Name = "Building";

        Card buildingCard = new Card(building);
        float dmg = 5f;
        
        _land.AddCard(buildingCard);
        
        _land.MakeDamage(dmg);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life - dmg);
    }
    
    [Test]
    public void CanMakeDamage()
    {
        CardData building = ScriptableObject.CreateInstance<CardData>();
        building.behaviour = CardBehaviourEnum.PRODUCTION_GOLD;
        building.type = CardTypeEnum.BUILDING;
        building.life = 10f;
        building.Name = "Building";

        Card buildingCard = new Card(building);
        
        _land.AddCard(buildingCard);
        
        CardData attack = ScriptableObject.CreateInstance<CardData>();
        attack.behaviour = CardBehaviourEnum.DAMAGE;
        attack.type = CardTypeEnum.ATTACK;
        attack.damage = 5f;
        attack.Name = "Attack";

        Card attackCard = new Card(attack);
        
        _land.AddCard(attackCard);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life - attackCard.GetDamage());
    }
    
    [Test]
    public void CanRevive()
    {
        CardData building = ScriptableObject.CreateInstance<CardData>();
        building.behaviour = CardBehaviourEnum.REVIVE;
        building.type = CardTypeEnum.BUILDING;
        building.life = 10f;
        building.Name = "Building";

        Card buildingCard = new Card(building);
        
        _land.AddCard(buildingCard);
        
        CardData attack = ScriptableObject.CreateInstance<CardData>();
        attack.behaviour = CardBehaviourEnum.DAMAGE;
        attack.type = CardTypeEnum.ATTACK;
        attack.damage = 5f;
        attack.Name = "Attack";

        Card attackCard = new Card(attack);
        
        _land.AddCard(attackCard);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life - attackCard.GetDamage());
        
        _land.AddCard(attackCard);
        
        Assert.False(_land.GetBuilding().IsDead());
        
        Assert.True(_land.GetBuilding().HasAlreadyRevived());
        
        
        _land.AddCard(attackCard);
        _land.AddCard(attackCard);
        
        Assert.Null(_land.GetBuilding());
    }
    
    [Test]
    public void CanAttack2Times()
    {
        CardData building = ScriptableObject.CreateInstance<CardData>();
        building.behaviour = CardBehaviourEnum.REVIVE;
        building.type = CardTypeEnum.BUILDING;
        building.life = 10f;
        building.Name = "Building";

        Card buildingCard = new Card(building);
        
        _land.AddCard(buildingCard);
        
        CardData attack = ScriptableObject.CreateInstance<CardData>();
        attack.behaviour = CardBehaviourEnum.RELENTLESS;
        attack.type = CardTypeEnum.ATTACK;
        attack.damage = 2f;
        attack.Name = "Attack";

        Card attackCard = new Card(attack);
        
        _land.AddCard(attackCard);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life - 2*attackCard.GetDamage());
        
    }
    
    [Test]
    public void CanAttackBlockFirstDamge()
    {
        CardData building = ScriptableObject.CreateInstance<CardData>();
        building.behaviour = CardBehaviourEnum.REVIVE;
        building.type = CardTypeEnum.BUILDING;
        building.life = 10f;
        building.Name = "Building";

        Card buildingCard = new Card(building);
        
        _land.AddCard(buildingCard);
        
        CardData absorb = ScriptableObject.CreateInstance<CardData>();
        absorb.behaviour = CardBehaviourEnum.ABSORB;
        absorb.type = CardTypeEnum.SUPPORT;
        absorb.Name = "Absorb";

        Card absorbCard = new Card(absorb);
        
        _land.AddCard(absorbCard);
        
        CardData attack = ScriptableObject.CreateInstance<CardData>();
        attack.behaviour = CardBehaviourEnum.DAMAGE;
        attack.type = CardTypeEnum.ATTACK;
        attack.damage = 5f;
        attack.Name = "Attack";

        Card attackCard = new Card(attack);
        
        _land.AddCard(attackCard);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life);
        
        _land.AddCard(attackCard);
        
        Assert.AreEqual(_land.GetBuilding().GetLife(), building.life - attack.damage);
        
    }
}
