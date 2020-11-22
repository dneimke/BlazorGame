# BlazorGame

A Blazor Server application that allows multiple players to play a game of snap across the internet.

A game is first created and given a PIN Code. 

![Creating a new game](https://github.com/dneimke/blazorgame/blob/master/docs/start-new-game.png?raw=true)

Other users can join a game by entering a username and the PIN Code.

![Multiple players in a game](https://github.com/dneimke/blazorgame/blob/master/docs/multiple-players.png?raw=true)

The game is started when any player deals the cards.

![Dealt cards](https://github.com/dneimke/blazorgame/blob/master/docs/dealt-cards.png?raw=true)

Each player takes turns to play a card. When a card is played, a player with the matching card can play their card to complete the turn.

Once started, the game state is maintained on the server and communicated to connected clients via a SignalR connection.
