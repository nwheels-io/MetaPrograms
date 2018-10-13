import axios from 'axios';

export class GreetingService {
    static getGreetingForName(name) {
        const data = {
            name
        };
        return axios.post("/api/greetingService/getGreetingForName", data).then(result => result.data).catch(err => console.log(err));
    }
}
