using System;
using Core.Code.Food;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public static class FoodTests
    {
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public static void TestRandomFoodGeneration(int testCount)
        {
            Random random = new Random();
            FoodDatabase foodDatabase = new FoodDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\foodDatabase.json");

            for (int test = 0; test < testCount; test++)
            {
                string foodId = foodDatabase.FoodSpawnRate.Evaluate((float) random.NextDouble());
                Assert.IsNotNull(foodId);
                Assert.IsNotEmpty(foodId);

                FoodDefinition foodDefinition = foodDatabase.FoodById[foodId];
                Assert.IsNotNull(foodDefinition);

                Food newFood = FoodUtilities.CreateFood(foodDefinition);
                Assert.IsNotNull(newFood);
                Assert.IsNotNull(newFood.Definition);
            }
        }
        
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public static void TestBasicRationGeneration(int testCount)
        {
            Random random = new Random();
            FoodDatabase foodDatabase = new FoodDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\foodDatabase.json");

            for (int test = 0; test < testCount; test++)
            {
                Food newFood = FoodUtilities.CreateBasicRation();
                Assert.IsNotNull(newFood);
                Assert.IsNotNull(newFood.Definition);
            }
        }
    }
}