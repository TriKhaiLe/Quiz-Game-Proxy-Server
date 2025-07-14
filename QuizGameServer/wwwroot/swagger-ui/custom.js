console.log("[Swagger Custom JS] Đã load file swagger-custom.js");

window.addEventListener('load', function () {
    const origFetch = window.fetch;
    window.fetch = function () {
        const [url, opts] = arguments;

        if (url.includes('/auth/login') && opts.method === 'POST') {
            return origFetch.apply(this, arguments).then(async res => {
                const clone = res.clone();
                const body = await clone.json();
                const token = body.access_token;

                if (token) {
                    // Gán token đúng chuẩn cho bearer
                    window.ui.authActions.authorize({
                        Bearer: {
                            name: "Bearer",
                            schema: {
                                type: "http",
                                in: "header",
                                scheme: "bearer",
                                bearerFormat: "JWT"
                            },
                            value: token
                        }
                    });

                    console.log("Token đã gán tự động cho Swagger:", token);
                }

                return res;
            });
        }

        return origFetch.apply(this, arguments);
    };
});
