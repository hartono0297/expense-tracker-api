window.addEventListener("load", async () => {
    console.log("✅ custom.js loaded");

    let token = null;

    try {
        const response = await fetch("/api/Auth", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: "tonok",
                password: "tonok"
            })
        });

        const result = await response.json();
        console.log("🔎 Auth response JSON:", result);

        token = result?.data?.token;
        console.log("🔑 Extracted token:", token);

        if (!token) {
            console.warn("⚠ No token found in response at data.token");
        }
    } catch (err) {
        console.error("❌ Auto-login failed", err);
    }

    // Initialize Swagger UI after token is fetched
    const ui = SwaggerUIBundle({
        url: "/swagger/v1/swagger.json",
        dom_id: "#swagger-ui",
        presets: [SwaggerUIBundle.presets.apis],
        layout: "BaseLayout",
        onComplete: function() {
            if (token) {
                // 👇 must match the Id = "Bearer" from Program.cs
                ui.preauthorizeApiKey("Bearer", "Bearer " + token);
                console.log("✅ Swagger auto-authorized with token");
            }
        }
    });
});
