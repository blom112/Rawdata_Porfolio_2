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
    
    let getNewTitles = (callback) => {
        fetch("api/title/newTitles")
            .then(response => response.json())
            .then(json => callback(json));
    };

    let getFavTitles = (uid, callback) => {
        fetch("api/title/favTitles")
            .then(response => response.json())
            .then(json => callback(json));
    };

    let getTrTitles = (callback) => {
        fetch("api/title/trTitles")
            .then(response => response.json())
            .then(json => callback(json));
    };

    let getRandTitles = (callback) => {
        fetch("api/title/randTitles")
            .then(response => response.json())
            .then(json => callback(json));
    };

    return {
        getCharsFromTitle,
        register,
        login,
        getNewTitles,
        getFavTitles,
        getTrTitles,
        getRandTitles
    }
});