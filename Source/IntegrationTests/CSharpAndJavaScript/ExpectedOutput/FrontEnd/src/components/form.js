import { app, h } from 'hyperapp';

const TX_STATUS_INPUT = 0;
const TX_STATUS_WAITING = 1;
const TX_STATUS_SUCCESS = 2;
const TX_STATUS_FAILURE = 3;

export class Form {

    static createState() {
        return {
            status: TX_STATUS_INPUT,
            data: null,
            error: null
        };
    }

    static createActions() {
        return {
            initData: data => { return { data } }, 
            setStatus: status => { return { status } },
            setInput: ({ setter, newValue }) => (state, actions) => {
                var newData = Object.assign({}, state.data);
                setter(newData, newValue);
                return { data: newData };
            },
            beginSubmit: (onSubmitting) => async (state, actions) => {
                actions.setStatus(TX_STATUS_WAITING);
                try {
                    await onSubmitting(state.data);
                    actions.endSubmit(null);
                } catch (err) {
                    actions.endSubmit(err);
                }
            },
            endSubmit: (error) => { 
                return {
                    error, 
                    status: (!!error ? TX_STATUS_FAILURE : TX_STATUS_SUCCESS)
                };
            }
        }; 
    }

    static component({ scopeSelector, id, data }, children) {
        return (state, actions) => (
            <form id={id} onsubmit={e => { e.preventDefault(); return false; }}
                oncreate={element => scopeSelector(actions).initData(data)}>
                {children}
            </form>
        );
    }

    static field({ scopeSelector, formId, propName, label, getter, setter, validation }) { 
        return (state, actions) => (
            <div key={`${formId}_${propName}`}>
                <label 
                    key="label"
                    for={`${formId}_${propName}__input`}>{label}</label>
                <input 
                    key="input"
                    id={`${formId}_${propName}__input`} 
                    name={propName} 
                    type="text" 
                    value={getter(scopeSelector(state).data)} 
                    readonly={(validation && validation.isOutput ? true : undefined)}
                    required={(validation && validation.required ? true : undefined)} 
                    maxLength={(validation && validation.maxLength ? validation.maxLength : undefined)} 
                    oninput={e => scopeSelector(actions).setInput({ setter, newValue: e.target.value })} />
            </div>
        );
    }

    static submit({ scopeSelector, onSubmitting }) {
        return (state, actions) => (
            <div key="Submit">
                <input 
                    type="submit" 
                    onclick={() => scopeSelector(actions).beginSubmit(onSubmitting)} />
            </div>
        );
    }
}
