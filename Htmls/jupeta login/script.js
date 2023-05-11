const form = document.getElementById("loginForm");

form.addEventListener("submit", function(event) {
    event.preventDefault(); //prevent the form from submitting normally
    
    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;
    
    //send a POST request to your API endpoint
    fetch('https://localhost:7172/api/User/Login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'accept': '*/*' 
      },
      body: JSON.stringify({ 
        email: email,
        passwordHash: password
      })
    })
    .then(response => {
        if (!response.ok) {
          throw new Error("Error: " + response.status);
        }
        return response.json();
      })
      .then(data => {
        //if authentication is successful, redirect to a protected page
        window.location.href = "home.html";

        //alert if login is successful
        alert("Login successful😁");
        //save the token to local storage
        localStorage.setItem("token", data.Token);
        localStorage.setItem("userEmail", data.Email);

    })
      .catch(error => {
        console.error("Error:", error);
        //display error message to user
        const errorElement = document.getElementById("error-message");
        errorElement.textContent = "Invalid email or password";
      });
  });