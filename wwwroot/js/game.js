
class Game {
    aspects = ['Vision', 'Product Ownership', 'Definition of Done', 'Velocity'];
    sentiments = ['Positive', 'Negative'];
    _hasChanges = false;
    
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

    async configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
        let self = this;

        connection.on("GameCreated", function (message) {
            console.info(message);
            self._hasChanges = true;
        })

        connection.on("PlayerJoined", function (message) {
            console.info(message);
            self._hasChanges = true;
        })

        connection.on("PlayerRetired", function (message) {
            console.info(message);
            self._hasChanges = true;
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
        console.info(game.HasChanges)
        if (game.HasChanges) {
            console.info('Game has changes')
            document.querySelector("#refresh-button").click();
            game.HasChanges = false;
        }
    }, 1000);
}