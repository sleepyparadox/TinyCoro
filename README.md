# TinyCoro
A simple coroutine class, intended for use in Unity
It iterates through a collection of ```Func<bool> ``` only fetching the next item when the current ```Func<bool>```'s condition is met or current is null

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

Create a simple IEnumerable method
```cs
//Print out a number each frame
public IEnumerator DoPrintEachFrame(int fromInclusive = 0, int toInclusive = 2)
{
	for (int i = fromInclusive; i <= toInclusive; i++)
	{
		Debug.Log(i);
		yield return null;
	}
}
```


Create the coroutine
```cs
TinyCoro.SpawnNext(DoPrintEachFrame);
```
This will print out ```0, 1, 2```


Create an anonymous method to insert arguments
```cs
TinyCoro.SpawnNext(() => DoPrintEachFrame(3, 7));
```
This will print out ```3, 4, 5, 6, 7```


Waiting on conditions
----------------
Yielding a ```Func<bool>``` will pause a coroutine until that condition is met

Yielding a ```TinyCoro.WaitUntil(Func<bool> condition);``` makes the code more readable and less error prone as c# lets you yield any object
``TinyCoro.WaitUntil``` simply returns the ```Func<bool>``` passed in but any decent compiler will optimise it out

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

Extra
----------------
```cs
var cutsceneCoro = TinyCoro.SpawnNext(() => ShowCutscene("DramaticIntro.mov"));

//Listen for when coroutine is finished
cutsceneCoro.OnFinished += (coro, reason) => Debug.Log(string.Format("{0} stopped because {1}", coro.Name, reason));

//Check if coroutine is still running
Debug.Log(cutsceneCoro.Alive);

//Stop a coroutine early
cutsceneCoro.Kill();
```


TODO:
Rip out the unity dependency in ```cs TinyCoro.Wait(float seconds) ```
Allow coroutines to be iterated independently