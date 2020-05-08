﻿// library functions
function getSeason(date) {
    // javascript Date.getMonth() is STUPID, and returns the 0-based index of the month rather than the month,
    // so checking for "Is it April?" means checking if getMonth() == 3
    // NOTE: If we're in April 2020, the season is actually part of the May 2020 season, so "202005". Otherwise it's
    // just yyyyMM for whatever month you're in.
    if (date.getUTCMonth() == 3 && date.getUTCFullYear() == 2020) {
        date.setMonth(4);
    }
    return d3.timeFormat("%Y%m")(date);
};

let hashParams = null;
function hashParam(name) {
    if (!hashParams) {
        hashParams = {};
        document.location.hash.substring(1).split("&").forEach(function (paramStr) {
            let kvp = paramStr.split("=");
            hashParams[kvp[0]] = kvp[1];
        });
    }
    return hashParams[name];
};

function param(name) {
    if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
        return decodeURIComponent(name[1]);
};

// constants
const API_URL = "http://sotledger.azurewebsites.net/api/";
const USER_URL = API_URL + "user/";
const LEDGER_URL = API_URL + "ledger";
const FACTIONS = {
    "af": {
        name: "Athena's Fortune",
        rgb: d3.rgb("#91E6CB")
    },
    "gh": {
        name: "Gold Hoarders",
        rgb: d3.rgb("#C3922E")
    },
    "ma": {
        name: "Merchant Alliance",
        rgb: d3.rgb("#9599B3")
    },
    "os": {
        name: "Order of Souls",
        rgb: d3.rgb("#764B74")
    },
    "rb": {
        name: "Reapers Bones",
        rgb: d3.rgb("#B13314")
    }
};

// global variables
let seasonData = { title: "Sea of Thieves Top Quartile Cut-offs for all five factions", factions: [], dates: [] };
let season = hashParam("season") || getSeason(new Date());
let user = null;
if (hashParam("user")) user = hashParam("user");

function refreshData() {
    $.ajax({
        async: true,
        type: "GET",
        url: LEDGER_URL + "?season=" + season,
        headers: {
            Accept: "application/json"
        },
        success: function (data) {
            seasonData.minScore = Number.MAX_VALUE;
            seasonData.maxScore = 0;
            seasonData.factions = [];
            let rowKeys = Object.keys(data.entries).sort();
            Object.keys(FACTIONS).forEach(function (facKey) {
                let facData = {
                    name: FACTIONS[facKey].name,
                    rgb: FACTIONS[facKey].rgb,
                    values: []
                };
                rowKeys.forEach(function (rowKey) {
                    let val = data.entries[rowKey][facKey + "_0_lo_score"];
                    if (val < seasonData.minScore) seasonData.minScore = val;
                    if (val > seasonData.maxScore) seasonData.maxScore = val;
                    facData.values.push(val);
                });
                seasonData.factions.push(facData);
            });
            seasonData.dates = rowKeys.map(d3.timeParse("faction-%Y%m%d-%H"));
            refreshUserData();
        }
    });
};

function refreshUserData() {
    if (user) {
        $.ajax({
            async: true,
            type: "GET",
            url: USER_URL + user + "?season=" + season,
            headers: {
                Accept: "application/json"
            },
            success: function (data) {
                let keys = Object.keys(data.entries).sort();
                let last = data.entries[keys[keys.length - 1]];
                Object.keys(FACTIONS).forEach(function (facKey) {
                    userData[facKey] = last[facKey + "_score"];
                });
                console.log(userData);
            }
        });
    }
    chart();
};

let margin = { top: 10, right: 80, bottom: 30, left: 80 },
    width = 900 - margin.left - margin.right,
    height = 500 - margin.top - margin.bottom;

let x = d3.scaleTime().range([0, width]);
let y = d3.scaleLinear().range([height, 0]);

let xAxis = d3.axisBottom().scale(x).ticks(5);
let yAxis = d3.axisLeft().scale(y).ticks(5);
let yAxisR = d3.axisRight().scale(y).ticks(5);

let scoreline = d3.line()
    .x(function (d, i) { return x(seasonData.dates[i]); })
    .y(function (d) { return y(d); });

function chart() {
    x.domain(d3.extent(seasonData.dates, d => d));
    y.domain([seasonData.minScore, seasonData.maxScore]);

    seasonData.factions.forEach(function (faction) {
        svg.append("path")
            .attr("class", "line")
            .style("stroke", faction.rgb)
            .attr("d", scoreline(faction.values));
    });

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis);
    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis);
    svg.append("g")
        .attr("class", "y axis")
        .attr("transform", "translate(" + width + " ,0)")
        .call(yAxisR);
};

let svg = null;
$(document).ready(function () {
    let title = $("<h3></h3>").text(seasonData.title);
    $("body").append(title);
    svg = d3.select("body")
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
    refreshData();
});