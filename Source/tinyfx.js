const TX_STATE_INPUT = 'ready';
const TX_STATE_WORKING = 'working';
const TX_STATE_SUCCESS = 'success';
const TX_STATE_FAILURE = 'failure';

class ServiceProxy {
    constructor(url) {
        this._url = url;
    }
    async invoke(action, data) {
        const request = new Request(`${this._url}/${action}`);

        const requestHeaders = new Headers();
        requestHeaders.append('Content-Type', 'application/json');

        const requestInit = {
            method: 'POST',
            body: JSON.stringify(data),
            headers: requestHeaders,
            cache: 'no-cache'
        };

        const response = await fetch(request, requestInit);

        if (!response.ok) {
            throw Error(response.statusText);
        }

        return response.json();
    }
}

class FormFieldBuilder {
    constructor(name) {
        this._options = { name };
    }
    isOutput() {
        this._options.isOutput = true;
        return this;
    }
    get options() {
        return this._options;
    }
}

class FormBuilder {
    constructor() {
        this._options = { fields: [] };
    }
    bindModel(model) {
        this._options.model = model;
        return this;
    }
    bindElement(formElement) {
        this._options.formElement = formElement;
        return this;
    }
    field(name, builderCallback) {
        const builder = new FormFieldBuilder(name);
        builderCallback && builderCallback(builder);
        this._options.fields.push(builder);
        return this;
    }
    onSubmit(callback) {
        this._options.onSubmit = callback;
        return this;
    }
    buildComponent() {
        return new FormComponent(this._options);
    }
}

class FormField {
    constructor(ownerForm, { name, isOutput }) {
        this._ownerForm = ownerForm;
        this._name = name;
        this._isOutput = isOutput;
    }
    scatter(model) {
        const value = model[this._name];
        const fieldElement = this._ownerForm.getFieldElement(this);

        if (this._isOutput) {
            fieldElement.innerText = value;
            fieldElement.parentElement.style.display = (value ? 'block' : 'none');
        } else {
            fieldElement.value = value;
        }
    }
    gather(model) {
        if (!this._isOutput) {
            const fieldElement = this._ownerForm.getFieldElement(this);
            model[this._name] = fieldElement.value;
        }
    }
    hasValue(model) {
        const value = model[this._name];
        return (typeof value === 'string' && value.length > 0);
    }
    get name() {
        return this._name;
    }
    get isOutput() {
        return this._isOutput;
    }
}

class FormComponent {
    constructor({ model, formElement, fields, onSubmit }) {
        this._model = model;
        this._id = formElement.id;
        this._fields = fields.map(f => new FormField(this, f.options));
        this._onSubmit = onSubmit;

        this._state = TX_STATE_INPUT;
        this._error = null;

        this._submitButton = document.getElementById(`${this._id}__buttons_submit`);
        this._statusDiv = document.getElementById(`${this._id}__status`);
        this._statusTextSpan = document.getElementById(`${this._id}__status__static`);

        formElement.addEventListener('submit', evt => this._handleFormSubmit(evt));

        this.scatter();
    }
    scatter() {
        this._fields.forEach(f => {
            if (f.isOutput) {
                const fieldContainer = document.getElementById(`${this._id}_${f.name}`);
                const isVisible = (this._state === TX_STATE_SUCCESS && f.hasValue(this._model));
                fieldContainer.style.display = (isVisible ? 'block' : 'none');
                if (!isVisible) {
                    return;
                }
            }
            f.scatter(this._model);
        });
        this._statusDiv.style.display = (this._state === TX_STATE_WORKING || this._state === TX_STATE_FAILURE ? 'block' : 'none');
        this._statusTextSpan.innerText = this.getStatusText();
    }
    gather() {
        this._fields.forEach(f => f.gather(this._model));
    }
    getStatusText() {
        switch (this._state) {
            case TX_STATE_WORKING: return 'WORKING...';
            case TX_STATE_FAILURE: return 'ERROR: ' + this._error;
            default: return '';
        }
    }
    getFieldElement(field) {
        const elementIdSuffix = (field.isOutput ? 'static' : 'input');
        const fieldElementId = `${this._id}_${field.name}__${elementIdSuffix}`;
        const fieldElement = document.getElementById(fieldElementId);
        return fieldElement;
    }
    _handleFormSubmit(evt) {
        this._executeTransaction();
        evt.preventDefault();
        return false;
    }
    _executeTransaction() {
        this.gather();
        this._state = TX_STATE_WORKING;
        this._error = null;
        this.scatter();

        let requestData = {};

        this._onSubmit().then(() => {
            this._state = TX_STATE_SUCCESS;
        })
            .catch(err => {
                this._state = TX_STATE_FAILURE;
                this._error = err.message;
            })
            .finally(() => this.scatter());
    }
}

export function CreateTransactionFormBuilder(builderCallback) {
    return new FormBuilder();
}

export function CreateServiceProxy(url) {
    return new ServiceProxy(url);
}
