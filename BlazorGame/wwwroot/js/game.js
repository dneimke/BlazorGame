
class Game {
    _razorServer;
    _signalR;

    constructor() {
        this.configureConnection();
    }
    
    GetConnectionId() {
        if (!this._signalR) {
            this.configureConnection();
        }
        return this._signalR.connectionId;
    }

    async InitializeGameState(server) {
        this._razorServer = server;
    }
    
    configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
        
        connection.on("GameCreated", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("PlayerJoined", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("PlayerRetired", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("GameStateChanged", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))

        this._signalR = connection;
        connection.start().then(x => console.info('Connection started'));
    }
}

const game = new Game();
window.Game = game;
