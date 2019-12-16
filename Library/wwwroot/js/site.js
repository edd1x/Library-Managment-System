// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const cardID = document.getElementById('cardID')
const button = document.getElementById('button')

button.addEventListener('submit', (e) => {
    if (cardID.value === '' || cardID.value == null) {
        messages.push('CardID is required')
    }
})


