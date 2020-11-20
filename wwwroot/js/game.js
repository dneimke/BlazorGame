
StartTimer = () => { console.info('Starting game loop') }

class Game {
    aspects = ['Vision', 'Product Ownership', 'Definition of Done', 'Velocity'];
    sentiments = ['Positive', 'Negative'];
    
    constructor(connection) {
        this.connection = connection;
        this.state = new GameState();
        this.players = []; // Player[]
    }

    addPlayer(player) {
        this.players.push(player);
    }

    async GetConnectionId() {
        if (!this.connection) {
            await this.configureConnection();
        }
        return this.connection.connectionId;
    }

    async configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

        connection.on("GameCreated", function (message) {
            console.info(message);
        })

        connection.on("PlayerJoined", function (message) {
            console.info(message);
        })

        connection.on("PlayerRetired", function (message) {
            console.info(message);
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

//const p1 = new Player('Darren');
//const p2 = new Player('Sahan');
//const p3 = new Player('Brad');

//p1.joinGame(game);
//p2.joinGame(game);
//p3.joinGame(game);

//game.deal();