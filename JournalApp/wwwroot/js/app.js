window.appFunctions = {
    hideKeyboard: function () {
        if (document.activeElement instanceof HTMLElement) {
            document.activeElement.blur();
        }
    },
    downloadFileFromStream: async function (fileName, contentStreamReference) {
        const arrayBuffer = await contentStreamReference.arrayBuffer();
        const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? 'export.pdf';
        anchorElement.click();
        anchorElement.remove();
        URL.revokeObjectURL(url);
    },
    toggleTheme: function (isLight) {
        if (isLight) {
            document.body.classList.add('light-mode');
            document.body.classList.remove('dark-mode');
            localStorage.setItem('theme', 'light');
        } else {
            document.body.classList.add('dark-mode');
            document.body.classList.remove('light-mode');
            localStorage.setItem('theme', 'dark');
        }
    },
    initializeTheme: function () {
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'light') {
            document.body.classList.add('light-mode');
            return true;
        } else {
            document.body.classList.add('dark-mode');
            return false;
        }
    }
};

function base64ToBlob(base64, type) {
    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return new Blob([bytes], { type: type });
}
