define(["knockout", "postman", 'dataService'], function (ko, postman, ds) {
    let usernameSignup = ko.observable();
    let passwordSignup = ko.observable();
    let emailSignup = ko.observable();
    let dobSignup = ko.observable();
    let usernameLogin = ko.observable();
    let passwordLogin = ko.observable();

    let currentView = ko.observable('Frontpage');

    let changeContent = menuItem => {
        console.log(menuItem);
        currentView(menuItem.component)
    };

    let isActive = menuItem => {
        return menuItem.component === currentView() ? "active" : "";
    }

    postman.subscribe("changeView", function (data) {
        currentView(data);
    });

    let register = () => {
        let user = {
            Username: usernameSignup(),
            Password: passwordSignup(),
            Email: emailSignup(),
            DateOfBirth: dobSignup()
        };

        ds.register(user, data => {
            console.log(data);
        });
        location.reload();
    };

    let login = () => {
        let user = {
            Username: usernameLogin(),
            Password: passwordLogin()
        };

        localStorage.removeItem("username");
        localStorage.removeItem("token");
        localStorage.removeItem("id");

        ds.login(user, data => {
            console.log(data);
            localStorage.setItem("username", data.username);
            localStorage.setItem("token", data.token);
            localStorage.setItem("id", JSON.parse(atob(data.token.split('.')[1])).id);
        });
    };

    let logout = () => {
        localStorage.removeItem("username");
        localStorage.removeItem("token");
        localStorage.removeItem("id");
        postman.publish("changeView", "Frontpage");
        location.reload();
    };

    return {
        currentView,
        changeContent,
        isActive,
        usernameSignup,
        passwordSignup,
        emailSignup,
        dobSignup,
        usernameLogin,
        passwordLogin,
        register,
        login,
        logout
    }
});