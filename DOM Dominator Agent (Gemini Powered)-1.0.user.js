// ==UserScript==
// @name         DOM Dominator Agent (Gemini Powered)
// @namespace    http://tampermonkey.net/
// @version      1.0
// @description  Adds a floating AI agent to help debug/RE any website or web game.
// @author       YourName
// @match        *://*/*
// @grant        GM_xmlhttpRequest
// @grant        GM_setValue
// @grant        GM_getValue
// @connect      generativelanguage.googleapis.com
// ==/UserScript==

(function() {
    'use strict';

    // --- CONFIGURATION ---
    // We use GM_getValue to store the key so you don't have to hardcode it.
    // The first time you run this, it will prompt you for the key.
    let API_KEY = GM_getValue("GEMINI_API_KEY", "");
    if (!API_KEY) {
        API_KEY = prompt("Enter your Gemini API Key to enable the Debug Agent:");
        if (API_KEY) GM_setValue("GEMINI_API_KEY", API_KEY);
    }

    // --- UI CREATION ---
    const container = document.createElement('div');
    container.style.cssText = `
        position: fixed; bottom: 20px; right: 20px; width: 300px;
        background: #1e1e1e; color: #00ff41; border: 2px solid #00ff41;
        z-index: 99999; font-family: monospace; padding: 10px;
        border-radius: 8px; box-shadow: 0 0 10px rgba(0,255,65,0.2);
    `;

    const title = document.createElement('div');
    title.innerText = "> AGENT_DEBUGGER_V1";
    title.style.fontWeight = "bold";
    title.style.marginBottom = "10px";
    title.style.cursor = "move"; // Hint that it could be draggable (logic not included for brevity)

    const inputArea = document.createElement('textarea');
    inputArea.placeholder = "Ex: How do I change the score? / Where is the login function?";
    inputArea.style.cssText = `
        width: 100%; height: 60px; background: #333; color: white;
        border: 1px solid #555; margin-bottom: 10px; resize: none;
    `;

    const analyzeBtn = document.createElement('button');
    analyzeBtn.innerText = "RUN ANALYSIS";
    analyzeBtn.style.cssText = `
        width: 100%; background: #00ff41; color: black; border: none;
        padding: 5px; cursor: pointer; font-weight: bold;
    `;

    const outputArea = document.createElement('div');
    outputArea.style.cssText = `
        margin-top: 10px; font-size: 12px; color: #ddd; max-height: 150px;
        overflow-y: auto; white-space: pre-wrap;
    `;

    container.appendChild(title);
    container.appendChild(inputArea);
    container.appendChild(analyzeBtn);
    container.appendChild(outputArea);
    document.body.appendChild(container);

    // --- LOGIC ---
    analyzeBtn.onclick = async () => {
        const userQuery = inputArea.value;
        if (!userQuery) return;

        outputArea.innerText = "Scanning DOM... Extracting scripts... Contacting Gemini...";

        // 1. GATHER CONTEXT
        // We grab the body HTML, but truncate it to avoid token limits.
        // We also specifically look for script tags.
        const bodyPreview = document.body.innerHTML.substring(0, 15000); // 15k char limit
        const scripts = Array.from(document.querySelectorAll('script'))
                             .map(s => s.src || "Inline Script")
                             .join('\n');

        // 2. CONSTRUCT PROMPT
        const prompt = `
            You are an expert JavaScript Reverse Engineer and Debugger.
            Current URL: ${window.location.href}
            User Query: "${userQuery}"

            Here is a summary of the page HTML and Scripts:
            SCRIPTS DETECTED:
            ${scripts}

            HTML PREVIEW (Truncated):
            ${bodyPreview}

            Based on this, provide a specific JavaScript snippet I can run in the console to achieve the user's goal.
            If you need to inspect a specific variable, tell me which one.
        `;

        // 3. CALL API (Using GM_xmlhttpRequest to bypass CORS)
        GM_xmlhttpRequest({
            method: "POST",
            url: `https://generativelanguage.googleapis.com/v1beta/models/gemini-3-pro-preview:generateContent?key=${API_KEY}`, // model
            headers: { "Content-Type": "application/json" },
            data: JSON.stringify({
                contents: [{ parts: [{ text: prompt }] }]
            }),
            onload: function(response) {
                const data = JSON.parse(response.responseText);
                if (data.candidates && data.candidates[0].content) {
                    const reply = data.candidates[0].content.parts[0].text;
                    outputArea.innerText = reply;
                } else {
                    outputArea.innerText = "Error: " + JSON.stringify(data);
                }
            },
            onerror: function(err) {
                outputArea.innerText = "Request Failed: " + err;
            }
        });
    };
})();
