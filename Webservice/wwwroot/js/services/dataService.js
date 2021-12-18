define([], () => {
    let getCharsFromTitle = (id, callback) => {
        fetch("api/title/" + id + "/CharactersFromTitle")
            .then(response => response.json())
            .then(json => callback(json));
    };

    let register = (user, callback) => {
        let params = {
            method: "POST",
            body: JSON.stringify(user),
            headers: {
                "Content-Type": "application/json"
            }
        };
        fetch('api/register/register', params)
            .then(response => response.json())
            .then(json => callback(json));
    };

    let login = (user, callback) => {
        let params = {
            method: "POST",
            body: JSON.stringify(user),
            headers: {
                "Content-Type": "application/json"
            }
        };
        fetch('api/login/login', params)
            .then(response => response.json())
            .then(json => callback(json));
    };

    return {
        getCharsFromTitle,
        register,
        login
    }
});