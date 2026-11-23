export class UnauthorizedError extends Error{
    constructor(message: string = "Session expired"){
        super(message);
        this.name = "UnauthorizedError";
    }
}