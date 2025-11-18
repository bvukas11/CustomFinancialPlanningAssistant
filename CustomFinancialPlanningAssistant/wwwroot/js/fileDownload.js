// File download utility for Blazor
// Allows downloading files generated on the server to the client browser

function downloadFile(base64, filename, contentType) {
    const linkSource = `data:${contentType};base64,${base64}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = filename;
    downloadLink.click();
}
