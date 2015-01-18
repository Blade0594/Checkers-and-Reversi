This is a project I started in 2008 as part of my first Master's thesis. 

The scope was to implement a checkers and a reversi game and build an AI for them. 
I eneded up evolving the checkers AI using genetic algorithms.

You may use the code here in whatever form you would like.

Please be advised the way the Checkers game maintains information about the game board and searches for available moves,
is by far not the most efficient. I would write the code differently now...

As the project was later used in order to test some adaptive AI for the checkers game, there might be quick and dirty
code in some parts (that mainly outputs different things I needed for my experiments). They can usually be cleaned up rather quickly.
I just lost interest in doing so :).

Program's main features:
 1) WPF UI for the reversi game + reversi game engine;
 2) WPF UI for the checkers game + checkers game engine;
 3) ASP.NET UI for the checkers game;
 4) GameAI is a dll that could in theory be used for other games (you only need to implement the interfaces it exposes, in
 principle). It features the ABNegamax algorithm, the original MinMax algorithm and the POSM algorithm (an online learning 
 algorithm, which will try to adapt the search depth according to the opponent's skill level).
 
 There are some aditional projects (function learners) which were used to conduct some experiments. I left them here, 
 but I doubt they will be of any use for someone :). 
 
 If you'd like to use the project for some particular task or extend it in some way, please don't hesitate to 
 contact me with questions. I will happily reply and consult you on my old code, whenever I have some time :).
