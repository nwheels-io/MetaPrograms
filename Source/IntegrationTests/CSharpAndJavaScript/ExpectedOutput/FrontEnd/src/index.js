import "babel-polyfill";
import { app, h } from 'hyperapp';
import { Form } from './components/form';
import { GreetingService } from './services/greetingService';

const PageState = {
    model: {
        name: null,
        greeting: null
    },
    form: Form.createState()    
};

const PageActions = {
    getState: () => (state, actions) => state,
    replaceModel: (newModel) => (state, actions) => { return {model: newModel} },
    form: Form.createActions(),
    form_submitting: (model) => async (state, actions) => {
        let newModel = Object.assign({}, model);
        newModel.greeting = await GreetingService.getGreetingForName(newModel.name);
        actions.replaceModel(newModel);
    }
};

const PageView = ({ model }, actions) =>
    <Form.component scopeSelector={x => x.form} id="helloPage_form" data={model}>
        <Form.field scopeSelector={x => x.form} formId="helloPage_form" propName="name" label="Name" getter={m => m ? m.name : "???"} setter={(m, v) => m.name = v} validation={{required:true, maxLength:50}} />
        <Form.submit scopeSelector={x => x.form} onSubmitting={data => actions.form_submitting(data)} />
        <Form.field scopeSelector={x => x.form} formId="helloPage_form" propName="greeting" label="Server says:" getter={() => model.greeting} validation={{isOutput:true}} />
    </Form.component>;

const pageController = app(PageState, PageActions, PageView, document.body);
