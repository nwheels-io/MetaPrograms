(function() {

    let model = {
        name: '',
        greeting: null
    };

    function scatterModel() {
        document.getElementById('HelloPage_Form_Name').value = model.name;
        document.getElementById('HelloPage_Form_Greeting').innerText = model.greeting;
    }

    function gatherModel() {
        model.name = document.getElementById('HelloPage_Form_Name').value;
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.getElementById('HelloPage_Form').addEventListener('submit', () => {
            const apiParams = {
                name: model.name
            };
            const fetchOptions = {
                method: 'POST',
                body: JSON.stringify(apiParams),
                headers: new Headers({
                    'Content-Type': 'application/json'
                })
            };
            fetch('api/getGreetingForName', fetchOptions)
                .then((response) => {
                    model.greeting = response.json();
                    scatterModel();
                })
                .catch((error) => {
                    alert(error);
                });
        });
    });

})();