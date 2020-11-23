
class Game {
    _hasChanges = false;
    _context;
    _dealButton = null;
    _server;
    _connection = null;
    
    constructor() {
        this.state = new GameState();
        this.players = []; // Player[]
    }

    addPlayer(player) {
        this.players.push(player);
    }

    get HasChanges() { return this._hasChanges; }
    set HasChanges(val) { this._hasChanges = val; }

    async GetConnectionId() {
        if (!this._connection) {
            await this.configureConnection();
        }
        return this._connection.connectionId;
    }

    async InitializeGameState(currentSession, dealButton, server) {
        let self = this;
        this._server = server;
        this._context = currentSession;
        try {

            this._dealButton = dealButton
            this._dealButton.addEventListener('click', async (e) => {
                await self._connection.invoke("DealCards", self._context.pinCode);
            });

            await this._connection.invoke("RequestGameState", currentSession.pinCode);
        } catch (err) {
            console.error(err);
        }
    }
    
    setGameState(state) {
        state.hands.forEach(hand => {
            let list = document.getElementById(`hand_${hand.userId}`);
            if (list) {
                let items = hand.cards.reduce((acc, card) => {
                    return acc + `<li class="list-inline-item">
                        <span class="badge badge-${card.color} mr-3" style="font-size: 1.5em">${card.name} ${card.icon}</span>
                    </li>`;
                }, '');
                list.innerHTML = items;
            }
        });          
        
        this._dealButton.style.display = state.canDealCards ? 'block' : 'none';
    }

    async configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
        let self = this;

        connection.on("GameCreated", function (message) {
            self._server.invokeMethodAsync('RefreshGame');
        })

        connection.on("PlayerJoined", function (message) {
            self._server.invokeMethodAsync('RefreshGame');
        })

        connection.on("PlayerRetired", function (message) {
            self._server.invokeMethodAsync('RefreshGame');
        })

        connection.on("GameState", function (message) {
            self.setGameState(message);
        })

        await connection.start();
        this._connection = connection;        
    }
}

class GameState {
    constructor() {
        this.currentTurn = null;
        this.isPlaying = false;
    }

    load(state) {
        this.currentTurn = state.turn;
        this.isPlaying = state.isPlaying;
    }
}

class Player {
    constructor(name) {
        this.name = name;
        this.hand = []; // Card[]
    }

    joinGame(game) {
        game.addPlayer(this);
    }

    show() {
        console.info(`${this.name} has ${this.hand.length} cards.`)
    }
}

class Card {
    constructor(name, sentiment) {
        this.name = name;
        this.sentiment = sentiment;
        this.isPlayed = false;
    }

    play() {
        this.isPlayed = true;
    }
}

const game = new Game();
window.Game = game;
