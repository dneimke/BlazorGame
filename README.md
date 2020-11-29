# BlazorGame

![Build and deploy ASP.Net Core app to Azure Web App - blazor-game](https://github.com/dneimke/BlazorGame/workflows/Build%20and%20deploy%20ASP.Net%20Core%20app%20to%20Azure%20Web%20App%20-%20blazor-game/badge.svg)

This game is deployed here https://blazor-game.azurewebsites.net/

A Blazor Server application that allows multiple players to play a game of snap across the internet.

This is not a robust game, it's just a simple demo while I learn more about Blazor.

The following steps outline the flow of the game.

## Step 1 - Create New Game

A user can create game a new game by supplying a username and a PIN Code.

![Creating a new game](https://github.com/dneimke/blazorgame/blob/main/docs/start-new-game.png?raw=true)

## Step 2 - Invite Others

With the game created, an info panel is displayed with an invitation link and showing the PIN code for the game.

![An info panel displays the invitation link and the PIN code](https://github.com/dneimke/blazorgame/blob/main/docs/invite-others.png?raw=true)

## Step 3 - Join the Game

Other users join a game from the supplied link and enter the game by providing a username and entering the PIN Code.

![Multiple players in a game](https://github.com/dneimke/blazorgame/blob/main/docs/multiple-players.png?raw=true)

## Step 4 - Deal the cards

As soon as anybody else (other than the owner) joins the game, the owner can deal the cards.

![Deal button is present when players have joined](https://github.com/dneimke/blazorgame/blob/main/docs/deal.png?raw=true)

## Step 5 - The game owner leads with a card

After the cards have been dealt, each player can see only their own cards. Status messages show
which action is required. The following image shows the status message that is displayed to the user whose turn it is to play a card.

![Dealt cards](https://github.com/dneimke/blazorgame/blob/main/docs/dealt-cards.png?raw=true)

## Step 6 - Play a matching card

After the player whose turn it was to lead has played their card, whichever player has a matching card can play it. The following image shows the status when the game is waiting for a player with a matching Panda card to play.

![Waiting for matching card](https://github.com/dneimke/blazorgame/blob/main/docs/play-matching.png?raw=true)

After a matching card is played, the status updates to show who played the cards for that turn.

![Matched cards](https://github.com/dneimke/blazorgame/blob/main/docs/matched-cards.png?raw=true)

Players now take it in turns to lead and match cards until all cards have been exhaused from the hands.

## Step 7 - Play again

With all hands completed, the owner is presented with an option to `Play again`

![Play again](https://github.com/dneimke/blazorgame/blob/main/docs/play-again.png?raw=true)
