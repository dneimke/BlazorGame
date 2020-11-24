
class Game {
    _razorServer;
    _signalR;
    
    async GetConnectionId() {
        if (!this._signalR) {
            await this.configureConnection();
        }
        return this._signalR.connectionId;
    }

    async InitializeGameState(server, tb) {
        this._razorServer = server;
        await GetConnectionId();
    }
    
    async configureConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
        
        connection.on("GameCreated", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("PlayerJoined", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("PlayerRetired", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))
        connection.on("GameStateChanged", async (message) => await this._razorServer.invokeMethodAsync('RefreshGame'))

        this._signalR = connection;
        await connection.start();
    }
}

const game = new Game();
window.Game = game;
