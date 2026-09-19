# simple-ctl-model-checker
A simple model checker that does model checking of CTL formulae. Again, mostly a playground to me

The idea is to at some point make this both in C# and in F#, to get a hint of the flavor of the functional
cousin of the language I am used. Also, formal methods are kinda neat, so I think implementing the labeling
algorithm for model checking is nice and small enough in scope to be doable. Plus, potentially applicable
in some PoC scenarios, like a BPM workflow.

## The plan:
- Make some classes to define a CTL formula
- Make a visitor to visit it recursively (well, pattern matching more than a visitor). At first, we turn it into a string
- Make a visitor to retrieve the set of atoms of a given formula
- Make a class describilng a Kripke Model, with Atoms, States and Transitions
- Ideally that should be readonly. So make a builder to build a valid model
- (I suppose at some point I would like to import a model from a json, I have to think about how to do 
  that while keeping the model readonly)
- Write a simple function that ensures that a formula can be checked against a model
  (just ensure that the set of Atoms of the model include the atoms used in the formula)
- Finally implement the labeling algorithm one way or another.
- (bonus: make a parser that parses a string into a CTLformula - not necessary, but could be nice.
  if lazy. tht can be done with the aid of ANTLR or something similar)

All rather straightforward, besides the last point I suppose.

And in the end, do it all again in F#.

## About formulae
I want to support CTL formulae using standard propositional connectors (`!, /\, \/, ->, _|_`)
plus the usual temporal operators (`AG, EG, AF, EF, AX, EX, A[ U ], E[ U ]`).
The idea in the C# version is to create an absract class `CtlFormula`, which then has
three inheritors: `AtomicFormula`, `BinaryFormula`, `UnaryFormula`, `BottomFormula`. Atomic formulae are those
which are simply an atom. Binary formulae are made by two CTL formulae and a binary operator
(`/\, \/, ->, A[ U ], E[ U ]`). Unary formulae are made by a CTL formula and a unary operator
(`!, AG, EG, AF, EF, AX, EX`). I don't believe currently there is really a need to differentiate between propositinal
and temporary operators in this. Since the bottom is (the only) zeroary 'operator' we have, it is treated separately.

Visitors of the formula will then perform pattern matching on these. So basically something like this:

```csharp
public abstract class CtlVisitor<TResult>
{
    public TResult Visit(CtlFormula formula)
        => formula switch
    {
      AtomicFormula atomic => VisitAtomic(atomic),
      UnaryFormula unary => VisitUnary(unary),
      BinaryFormula binary => VisitBinary(binary),
      BottomFormula bottom => VisitBottom(bottom),
    };
    
    public abstract TRsult VisitAtomic(AtomicFormula atomic);
    // etc etc.
}
```

so inheritors can simply define what needs to be done in each branch of the pattern matching
by implementing `VisitAtomic`, `VisitUnary` etc.

## About models

Ideally I want to keep my model as stupid as possible. Something like
```csharp
public class Model
{
    public HashSet<string> Atoms {get;} // but I also would like for this to not be modifiable! hmmm...
    public List<State> States {get;} // also this, probably ReadonlyList or something
}

public class State
{
    public Guid Id {get;} = Guid.NewGuid();
    public string Name {get;}
    public HashSet<Guid> Transitions {get; } // or maybe even hashset of State? They would be pointers after all
    public HashSet<string> Atoms{ get; }
}
```

I would like for the user to be mostly unable to modify these, once an instance of `Model`
is created. So I would like to figure out a way to have something like a `ModelBuilder` which could be used
more or less like this
```csharp
var builder = new ModelBuilder();
bulder
    .AddAtom("p")
    .AddAtom("q");

builder
    .AddState("State 01")
    .WithAtom("p")
    .WithAtom("q")
    .SetInitial();

builder
    .AddState("State 02")
    .WithAtom("p")
    .WithSelfLoop();

builder
    .State("State 01")
    .HasTransistion("State 02");
// might as well not use guids to identify states at this point, I guess...

var model = builder.BuildModel(); // at this point, no modification to model should be possible

// In my head, this would make the model

// State01 (p,q) - initial
//   |
//   |
//   \/
// State02 (p)  (with a self loop)
```

## About model checking
Basically I want to improve a simple version of the labeling algorithm.As detailed in many resources online 
(such as [this](https://www.diag.uniroma1.it/degiacom/didattica/metodiformali/aa2008-09/materiale/6-modelchecking/slide5extended-4up.pdf) 
or [this](https://disi.unitn.it/rseba/DIDATTICA/fm2024/SLIDES/05-CTL-explicitstate_handouts.pdf)),
the idea is to compute *denotations* for CTL formulae, that is the sets of states in which they hold. We do that bottom up by induction on the syntax. 

That basically splits in three cases:
- for atoms, just see if that atom is in the state or not
- for X operators, compute pre-images
- For all other temporal operators, it is a fixpoint computation based on "tableaux rules" (see [here at slide 72](https://disi.unitn.it/rseba/DIDATTICA/fm2024/SLIDES/03-temporalLogics_slides.pdf)).
I'll think whether it's minimum or maximum fixpoint in each case when the time comes.

here are the tableaux rules. 
> After all... tomorrow is another day
```
AF(p) <-> p \/ AXAFp
EF(p) <-> p \/ EXEFp

AG(p) <-> p /\ AXAGp
EG(p) <-> p /\ EXEGp

A[pUq] <-> q \/ (p /\ AXA[pUq])
E[pUq] <-> q \/ (p /\ EXA[pUq])
```

Besides implementing minimum fixpoint (for `F` and `U`) and maximum fixpoint (for `G`) operators,
as well as preimage, this should all be easily accomplisheable by a simple visitor, at least in the C#
version of it
