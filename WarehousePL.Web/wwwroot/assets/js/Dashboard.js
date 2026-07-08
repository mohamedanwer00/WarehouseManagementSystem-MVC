

$(document).ready(function () {

    $.ajax({
        type: "GET",
        url: "/Voters/VoterChart",
        dataType: "json",
        success: function (data) {
            var polarChartElement = document.getElementById("polarChart2");
            var QuestionStatus = document.getElementById("QuestionStatus");
            InitPolarChart(polarChartElement, data.labels, data.values,'Person');
            InitPolarChart(QuestionStatus, data.labelsQuestion, data.valuesQuestion,'Person');
        },
        error: function (xhr, status, error) {
            console.error("Error fetching polar chart data:", error);
        }
    });


});