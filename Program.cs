namespace Event;

/*
Näin teet Eventin (class EventStuff)
1. tee delegaatti
2. tee event, joka käyttää delegaattia
3. tee metodi mitä huudellaan (esim. kaikki subaajat saa samalla tiedon)
*/

class EventStuff{ //julkaisijaluokka
    public delegate void EventDelegate(string msg); // Delegaatin on vastattava metodia: sekä delegaatti että siihen liitetyt metodit ottavat vastaan string-parametrin.
                                                    //(msg) => Program.ThisMethodWillRunIfEventIsTriggered("Hello from AnotherClass class"); //subscribe

    public event EventDelegate? EventBroadcaster; //tämä (EventBroadcaster) tilataan

    public void EventMethod(){ // Metodi, joka laukaisee tapahtuman – kutsuu kaikki tilaajat (subscribers)
        EventBroadcaster?.Invoke("Triggered!");
    }
}

class AnotherClass{
    EventStuff ES;
    public AnotherClass(EventStuff es)
    {
        ES = es;
        ES.EventBroadcaster += (msg) => Program.ThisMethodWillRunIfEventIsTriggered("Hello from AnotherClass class"); //tilaa event (subscribe)
    }
}
class AndOneMoreClass{
    EventStuff ES;
    public AndOneMoreClass(EventStuff es)
    {
        Console.WriteLine("AndOneMoreClass sub");
        ES = es;
        ES.EventBroadcaster += (msg) => Program.ThisMethodWillRunIfEventIsTriggered("Hello from AndOneMoreClass class"); //tilaa event (subscribe)
    }
}

class Program
{
    // Kaikki oliot staattisia, jotta niitä voidaan käyttää suoraan Main-metodissa ilman olion luontia
    static EventStuff es = new EventStuff(); //event object to be shared

    static AnotherClass ac = new AnotherClass(es); 
    static AndOneMoreClass aomc = new AndOneMoreClass(es); 

    static void Main(string[] args){
        //Lambdan käyttö helpottaa viestin/parametrin välitystä, mutta estää event tilauksen lopetuksen 
        //
        //Muut luokat käyttävät alla olevaa tyyliä
        //es.EventBroadcaster += (msg) => ThisMethodWillRunIfEventIsTriggered("Hello from Program class"); //tilaa event (subscribe)
        //
        //tehdään tässä luokassa tilaavasta metodista muuttuja (EventStuff.EventDelegate -delegaatti)
        //näin voimme perua lambdaMethod tilauksen halutessamme (*) 
        EventStuff.EventDelegate lambdaMethod = (msg) => ThisMethodWillRunIfEventIsTriggered("Hello from Program class");
        es.EventBroadcaster += lambdaMethod; //tilaa event (subscribe)

        //Console.WriteLine("Write init: ");
        //if(Console.ReadLine() == "init"){
            Console.WriteLine("Triggering the event from Program class");
            es.EventMethod(); // kutsutaan trigger eventiä 

            Console.WriteLine("--Again--");

            es.EventBroadcaster -= lambdaMethod; //peru eventin tilaus (unsubscribe) (*)
            es.EventMethod(); // kutsutaan trigger eventiä 
        //}
    }

    public static void ThisMethodWillRunIfEventIsTriggered(string msg){
        Console.WriteLine(msg);
    }
}
