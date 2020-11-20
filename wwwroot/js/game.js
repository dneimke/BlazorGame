
class Game {
    aspects = ['Vision', 'Product Ownership', 'Definition of Done', 'Velocity'];
    sentiments = ['Positive', 'Negative'];
    _hasChanges = false;
    _context;
    _dealButton = null;
    
    constructor(connection) {
        this.connection = connection;
        this.state = new GameState();
        this.players = []; // Player[]
    }

    addPlayer(player) {
        this.players.push(player);
    }

    get HasChanges() { return this._hasChanges; }
    set HasChanges(val) { this._hasChanges = val; }

    async GetConnectionId() {
        if (!this.connection) {
            await this.configureConnection();
        }
        return this.connection.connectionId;
    }

    async InitializeGameState(currentSession) {
        const self = this;
        this._context = currentSession;
        this._dealButton = document.getElementById('deal-button')
        this._dealButton.addEventListener('click', async (e) => {
            await self.connection.invoke("DealCards", currentSession.pinCode);
        });
        this._dealButton.style.display = 'none';
        try {
            await this.connection.invoke("RequestGameState", currentSession.pinCode);
        } catch (err) {
            console.error(err);
        }
    }
    
    setGameState(state) {
        state.hands.forEach(hand => {
            let list = document.getElementById(`hand_${hand.userId}`);
            if (list) {
                let items = hand.cards.reduce((acc, card) => {
                    console.info('card', card)
                    return acc + `<li class="list-inline-item">
                        <span class="badge badge-${card.color} mr-3" style="font-size: 1.5em">${card.name} ${card.icon}</span>
                    </li>`;
                }, '');
                list.innerHTML = items;
            }
        });  

        let style = state.canDealCards ? 'block' : 'none';
        console.info(style)
        this._dealButton.style.display = style;
    }

    async configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
        let self = this;

        connection.on("GameCreated", function (message) {
            self._hasChanges = true;
        })

        connection.on("PlayerJoined", function (message) {
            self._hasChanges = true;
        })

        connection.on("PlayerRetired", function (message) {
            self._hasChanges = true;
        })

        connection.on("GameState", function (message) {
            self.setGameState(message);
        })

        await connection.start();
        this.connection = connection;        
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
StartTimer = () => {
    console.info('Starting game loop');
    setInterval(function () {
        if (game.HasChanges) {
            try {
                document.getElementById("refresh-button").click();
            } catch { }            
            game.HasChanges = false;
        }
    }, 1000);
}