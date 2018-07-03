import axios from 'axios';

export class GreetingService {
    static getGreetingForName(parameters) {
        return axios
            .post('http://localhost:3300/api/index/getGreetingForName', parameters)
            .then(result => result.data)
            .catch(err => console.log(err));
    }
}
