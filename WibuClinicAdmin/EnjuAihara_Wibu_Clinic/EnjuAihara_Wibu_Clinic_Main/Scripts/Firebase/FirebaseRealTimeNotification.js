import { initializeApp } from 'https://www.gstatic.com/firebasejs/9.19.1/firebase-app.js';
import { getAuth, onAuthStateChanged } from 'https://www.gstatic.com/firebasejs/9.19.1/firebase-auth.js';
import { getDatabase, ref, child, get, onValue } from 'https://www.gstatic.com/firebasejs/9.19.1/firebase-database.js';
import { getFunctions, httpsCallable } from 'https://www.gstatic.com/firebasejs/9.19.1/firebase-functions.js';
const firebaseConfig = {
    apiKey: "AIzaSyBkyc7R6BV4cVg3DpL2RmgWKsuvuH5GoIM",
    authDomain: "quanlyphongmachwibu.firebaseapp.com",
    databaseURL: "https://quanlyphongmachwibu-default-rtdb.firebaseio.com",
    projectId: "quanlyphongmachwibu",
    storageBucket: "quanlyphongmachwibu.appspot.com",
    messagingSenderId: "702799829568",
    appId: "1:702799829568:web:dc8c8783b9dc6923229da5",
    measurementId: "G-CFDYH6PRLN"
};
//Config của firebase
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const db = getDatabase(app);
const starCountRef = ref(db, '/');
var flag = false;
function addNewNotification(Id) {
    var formdata = new FormData();
    formdata.append("Id", Id);
    $.ajax({
        type: "POST",
        url: "/Shared/GetNewNotification",
        data: formdata,
        processData: false,
        contentType: false,
        success: function (data) {
            $("#NotificationList").append(data);
        },
        error: function (event, xhr, options, exc) {
            console.log("Có lỗi với hệ thống firebase");
        }
    });
}
export function CheckChange() {
    onValue(starCountRef, (snapshot) => {
        if (flag == true) {
            var i = $("#NumberOfNotification").text();
            if (i == "" || i == null) {
                $("#NumberOfNotification").text("1");
            }
            else {
                $("#NumberOfNotification").text(parseInt(i) + 1);
            }
            var Id = Object.keys(snapshot.val())[0];
            if (Id != null && Id != '') {
                addNewNotification(Object.values(snapshot.val())[0]);
            }
        }
        else
            flag = true;
    });
    
}