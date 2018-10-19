using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Health_Tests {

    [Test]
    public void DealDamage()
    {
        GameObject hgo = new GameObject("Health");
        Health h = hgo.AddComponent<Health>();

        Vector3 res;

        // Basic, and return values.

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(100f, 0f);
        Assert.AreEqual(100f, h.CurrentHealth);
        Assert.AreEqual(100f * Health.ARMOUR_RESISTANCE, h.CurrentArmour);
        Assert.AreEqual(new Vector3(0f, 100f * Health.ARMOUR_RESISTANCE, 0f), res);

        h.Reset(100f, 100f, 100f, 0f);
        res = h.DealDamage(100f, 0f);
        Assert.AreEqual(0f, h.CurrentHealth);
        Assert.AreEqual(0f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(100f, 0f, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(200f, 0f);
        Assert.AreEqual(100f, h.CurrentHealth);
        Assert.AreEqual(0f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(0f, 200f * Health.ARMOUR_RESISTANCE, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(250f, 0f);
        Assert.AreEqual(50f, h.CurrentHealth);
        Assert.AreEqual(0f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(50f, 100f, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(300f, 0f);
        Assert.AreEqual(0f, h.CurrentHealth);
        Assert.AreEqual(0f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(100f, 100f, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(350f, 0f);
        Assert.AreEqual(0f, h.CurrentHealth);
        Assert.AreEqual(0f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(100f, 100f, 50f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(0f, 0f);
        Assert.AreEqual(100f, h.CurrentHealth);
        Assert.AreEqual(100f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(0f, 0f, 0f), res);

        // Using armour pen.

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(100f, 1f);
        Assert.AreEqual(0f, h.CurrentHealth);
        Assert.AreEqual(100f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(100f, 0f, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(175f, 1f);
        Assert.AreEqual(0f, h.CurrentHealth);
        Assert.AreEqual(100f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(100f, 0f, 75f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(0f, 1f);
        Assert.AreEqual(100f, h.CurrentHealth);
        Assert.AreEqual(100f, h.CurrentArmour);
        Assert.AreEqual(new Vector3(0f, 0f, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(100f, 0.5f);
        Assert.AreEqual(50f, h.CurrentHealth);
        Assert.AreEqual(100f - 50f * Health.ARMOUR_RESISTANCE, h.CurrentArmour);
        Assert.AreEqual(new Vector3(50f, 50f * Health.ARMOUR_RESISTANCE, 0f), res);

        h.Reset(100f, 100f, 100f, 100f);
        res = h.DealDamage(100f, 0.25f);
        Assert.AreEqual(75f, h.CurrentHealth);
        Assert.AreEqual(100f - 75f * Health.ARMOUR_RESISTANCE, h.CurrentArmour);
        Assert.AreEqual(new Vector3(25f, 75f * Health.ARMOUR_RESISTANCE, 0f), res);

        for (float i = 0; i < 100f; i++)
        {
            for (int j = 0; j <= 20; j++)
            {
                float p = j / 20f;

                h.Reset(100f, 100f, 100f, 100f);
                res = h.DealDamage(i, p);

                float exArmour = 100f - i * (1f - p) * Health.ARMOUR_RESISTANCE;
                float exHealth = 100f - (i * p);

                Assert.AreEqual(exHealth, h.CurrentHealth, 0.01f, "Health - Using damage {0} and pen {1}", i, p);
                Assert.AreEqual(exArmour, h.CurrentArmour, 0.01f, "Armour - Using damage {0} and pen {1}", i, p);
                Assert.AreEqual(100f - exHealth, res.x, 0.01f);
                Assert.AreEqual(100f - exArmour, res.y, 0.01f);
                Assert.AreEqual(0f, res.z, 0.01f);
            }
        }
    }
}
