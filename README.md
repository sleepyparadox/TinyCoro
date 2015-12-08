# TinyCoro
A simple coroutine class, intended for use in Unity
It iterates through a collection of ```cs Func<bool> ``` only fetching the next item when the current ```cs Func<bool>```'s condition is met or current is null

Getting Started
----------------
Create a MonoBehaviour that will step each active coroutine once per frame

```cs
public class MainBehaviour : MonoBehaviour
{
    public void Update()
    {
        TinyCoro.StepAllCoros();
    }
}
```

Creating a simple coroutine
----------------

Create a simple IEnumerable operation
```cs
//Print out a number each frame
public IEnumerator DoPrintEachFrame()
{
	for (int i = 0; i < 10; i++)
	{
		Console.WriteLine(i);
		yield return null; //Finished working for this frame
	}
}
```

Create the coroutine
```cs
//Start printing numbers each frame
TinyCoro.SpawnNext(DoPrintEachFrame);
```

Use an anonymous metho to add more arguments
```cs
//Print out a number each frame
public IEnumerator DoPrintEachFrame(int fromInclusive, int toInclusive)
{
	for (int i = fromInclusive; i <= toInclusive; i++)
	{
		Debug.Log(i);
		yield return null;
	}
}

//Start printing numbers each frame
TinyCoro.SpawnNext(() => DoPrintEachFrame(0, 10));
```

Waiting on conditions
----------------
Yielding a ```cs Func<bool>``` will pause a coroutine until that condition is met

TinyCoro.WaitUntil ```cs Func<bool>``` makes the code more readable and any decent compiler will compile it out

Here's an example of how you could create a quest
```cs
public class Quest
{
	public int ChickensCollected { get; set;}
	public int ChickensRequired { get; private set;}
	public long GoldToAward { get; private set;}
	public NPC QuestGiver { get; private set; }
	public Quest(NPC questGiver, int chickensToKill, long goldToAward)
	{
		QuestGiver = questGiver;
		ChickensRequired = chickensToKill;
		GoldToAward = goldToAward;
	}

	public void Start()
	{
		TinyCoro.SpawnNext(DoQuest);
	}

	public IEnumerator DoQuest()
	{
		Player.ShowQuestMessage(string.Format("Collect {0} Chickens", ChickensRequired));
		//Wait for Player to collect enough chickens
		yield return TinyCoro.WaitUntil(PlayerHasEnoughChickens);

		Player.ShowQuestMessage(string.Format("Return to {0}", QuestGiver.Name));
		//Wait for Player to return to Quest Giver
		yield return TinyCoro.WaitUntil(PlayerNearQuestGiver);

		//Cleanup UI messages
		Player.HideQuestMessage();
		Player.ShowMessageBox("Quest complete!");

		//Award Gold from Quest
		Player.Gold += GoldToAward;
	}

	private bool PlayerHasEnoughChickens()
	{
		return ChickensCollected >= ChickensRequired;
	}

	public bool PlayerNearQuestGiver()
	{
		return (QuestGiver.WorldPosition - Player.WorldPosition).Magnitude < 100;
	}
}

//Create and start a quest
var quest = new Quest(bob, 10, 100);
quest.Start();
```

TODO:
Rip out the unity dependency in ```cs TinyCoro.Wait(float seconds) ```
Allow coroutines to be iterated independently