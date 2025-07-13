console.log("[Swagger Custom JS] Đã load file swagger-custom.js");
window.addEventListener('load', function () {
  // Hook vào sự kiện hoàn thành gọi API
  const origFetch = window.fetch;
  window.fetch = function() {
    const [url, opts] = arguments;

    // Nếu là gọi API login
    if (url.includes('/auth/login') && opts.method === 'POST') {
      return origFetch.apply(this, arguments).then(async res => {
        const clone = res.clone(); // Clone để đọc body
        const body = await clone.json();
        const token = body.access_token;

        if (token) {
          // Gán token vào Swagger UI (giả lập bấm "Authorize")
          const auth = {
            Bearer: {
              name: "Authorization",
              schema: {
                type: "apiKey",
                in: "header",
                name: "Authorization"
              },
              value: "Bearer " + token
            }
          };
          window.ui.preauthorizeApiKey("Bearer", "Bearer " + token);
          console.log("Token đã gán tự động cho Swagger:", token);
        }

        return res;
      });
    }

    return origFetch.apply(this, arguments);
  };
});
