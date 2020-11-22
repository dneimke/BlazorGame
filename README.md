# BlazorGame

A Blazor Server application that allows multiple players to play a game of snap across the internet.

A game is first created and given a PIN Code. Other users can then join in-progress games by entering 
their username and the PIN Code.

The game is started by dealing the cards.

Each player takes turns to play a card. When a card is played, a player with the matching card can play their card to complete the turn.

Once started, the game state is maintained on the server and communicated to connected clients via a SignalR connection.
