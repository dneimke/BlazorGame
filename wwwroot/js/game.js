
StartTimer = () => { console.info('Starting game loop') }

class Game {
    constructor(connection) {
        this.connection = connection;
        this.foo = "Foo"
    }

    start() {
        alert('hello');
    }

    async GetConnectionId() {
        if (!this.connection) {
            await this.initializeConnection();
        }
        return this.connection.connectionId;
    }

    async initializeConnection() {
        const connection = new signalR.HubConnectionBuilder().withUrl("/sessionHub").build();
        await connection.start();
        this.connection = connection;        
    }
}

window.Game = new Game();