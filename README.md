# BlazorGame

![Build and deploy ASP.Net Core app to Azure Web App - blazor-game](https://github.com/dneimke/BlazorGame/workflows/Build%20and%20deploy%20ASP.Net%20Core%20app%20to%20Azure%20Web%20App%20-%20blazor-game/badge.svg)

This game is deployed here https://blazor-game.azurewebsites.net/

A Blazor Server application that allows multiple players to play a game of snap across the internet. NOTE: this is WIP.  Currently multiple players can join a session and deal cards but they cannot play the cards at this stage.

A game is first created and given a PIN Code. 

![Creating a new game](https://github.com/dneimke/blazorgame/blob/main/docs/start-new-game.png?raw=true)

Other users can join a game by entering a username and the PIN Code.

![Multiple players in a game](https://github.com/dneimke/blazorgame/blob/main/docs/multiple-players.png?raw=true)

The game is started when any player deals the cards.

![Dealt cards](https://github.com/dneimke/blazorgame/blob/main/docs/dealt-cards.png?raw=true)

Each player takes turns to play a card. When a card is played, a player with the matching card can play their card to complete the turn.

Once started, the game state is maintained on the server and communicated to connected clients via a SignalR connection.
