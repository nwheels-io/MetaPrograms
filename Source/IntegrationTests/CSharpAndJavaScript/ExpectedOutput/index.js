import { CreateTransactionFormBuilder, CreateServiceProxy } from './tinyfx.js';

document.addEventListener('DOMContentLoaded', () => {

    const model = {
        name: '',
        greeting: null
    };

    const greetingService = CreateServiceProxy('api/index');

    async function form_submitting() {
        model.greeting = await greetingService.invoke('getGreetingForName', { name: model.name });
        console.log(model);
    }

    const form = CreateTransactionFormBuilder()
        .bindModel(model)
        .bindElement(document.getElementById('helloPage_form'))
        .field('name')
        .field('greeting', f => f.isOutput())
        .onSubmit(form_submitting)
        .buildComponent();

});
