document.addEventListener("DOMContentLoaded", function () {
  // Get a reference to the sign-in button by its ID
  const signInButton = document.getElementById("signInButton");

  // Add a click event listener to the button
  signInButton
    .addEventListener("click", function () {
      //prevent the form from submitting normally

      let email = document.getElementById("email").value;
      let password = document.getElementById("password").value;

      //send a POST request to your API endpoint

      const url =
        "https://ec2-44-197-193-3.compute-1.amazonaws.com/api/User/Login";

      const data = {
        email: email,
        passwordHash: password,
      };

      fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error("Error: " + response.status);
          }
          return response.json();
        })
        .then((data) => {
          //if authentication is successful, redirect to a protected page
          window.location.href = "home.html";
          console.log("Success:", data);
          //display success message to user
          // const successElement = document.getElementById("success-message");
          // successElement.textContent = "Login successful";
          //redirect to home page
          //window.location.href = "home.html";
        });
      //alert if login is successful
      alert("Login successful😁");
      //save the token to local storage
      localStorage.setItem("token", data.Token);
      localStorage.setItem("userEmail", data.responseData.email);
    })
    .catch((error) => {
      console.error("Error:", error);
      //display error message to user
      const errorElement = document.getElementById("error-message");
      errorElement.textContent = "Invalid email or password";
    });
});
