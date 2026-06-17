# GitHub Copilot Fundamentals: Intro to GitHub Copilot

## 1. Overview & Architecture
*   **What is it?** An AI-powered pair programmer developed by **Microsoft** and **OpenAI**.
*   **Engine:** Powered by **OpenAI Codex** (a GPT-3 descendant trained on a massive concentration of public source code, making it superior to standard GPT-3 for code generation).
*   **Supported IDEs:** VS Code, Visual Studio, Vim/Neovim, JetBrains IDEs.
*   **Impact Metrics:** 
    *   **46%** of new code written by AI.
    *   **55%** faster overall developer productivity.
    *   **74%** of developers feel more focused.

---

## 2. GitHub Copilot Features
*   **Copilot Chat:** Interactive conversational interface in the IDE. Understands current code context to answer questions, explain errors/logic, and generate tests/docs.
*   **Pull Request Summaries:** Automatically generates change descriptions for pull requests on GitHub.
*   **Code Review Assistance:** Proposes code improvements, edge cases, and explains changes during reviews.
*   **Copilot for the CLI:** Suggests commands, writes shell scripts, explains terminal outputs, and generates projects *directly in the terminal*.
*   **Copilot Spaces:** Dedicated collaborative workspace for planning, requirement refinement, and iterating on designs.
*   **Copilot Cloud Agent:** Autonomous assistant executing multi-step coding workflows (e.g., generating multiple related files, building project scaffolding from specs).

---

## 3. Subscription Plans (Comparison)

| Feature / Plan | Copilot Free | Copilot Pro | Copilot Pro+ | Copilot Business | Copilot Enterprise |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **Target Audience** | Free Individuals | Paid Individuals | Power Individuals | Teams & Orgs | Large Enterprises |
| **Completions & Chat** | monthly limited | unlimited (high limits) | higher limit / capacity | managed access | enterprise managed |
| **Model Access** | Advanced models | Priority access | Priority premium | Secure / Managed | Custom fine-tuned |
| **IDE Integration** | Yes | Yes | Yes | Yes | Yes (with personalization) |
| **Policy Controls** | No | No | No | **Centralized** | **Centralized** |
| **Public Code Filter** | No | Yes (user setting) | Yes (user setting) | **Enforced Filter** | **Enforced Filter** |
| **IP Indemnity** | No | No | No | **Yes** | **Yes** |
| **Private Code Base Indexing** | No | No | No | No | **Yes (Enterprise Cloud)** |
| **Custom Models** | No | No | No | No | **Yes (Fine-tuning)** |

---

## 4. IDE Interaction & Triggers

### A. Inline Suggestions
*   **Trigger:** Automatically behaves as autocompletion as you type, rendering greyed-out ghost text.
*   **Shortcuts (Windows / macOS):**
    *   **Accept entire suggestion:** `Tab` or `>` (Right Arrow)
    *   **Reject suggestion:** `Esc` or continue typing
    *   **Cycle suggestions:** `Alt + ]` / `Option + ]` (forward), `Alt + [` / `Option + [` (backward)

### B. Comments-to-Code
*   Type a natural language comment explaining what you want (e.g., `# Function to reverse a string`) and press `Enter`. Copilot will generate the function body underneath.

### C. Inline Chat (`Ctrl + I` / `Cmd + I`)
*   Opens a chat input field directly at the cursor line.
*   Supports **slash commands** (e.g., `/explain` to explain a snippet, `/tests` to generate unit tests).

### D. Explanations & Test Generation
*   **Explain This:** Highlight code -> Right-click -> Select **Copilot: Explain This**.
*   **Unit Tests:** Highlight a function/class -> Use Command Palette -> **Copilot: Generate Unit Tests** (or use inline `/tests`).

---

## 5. Setup, Configuration & Troubleshooting (VS Code)

### A. Setup & Sign-In
1. Install **GitHub Copilot** extension from the VS Code Marketplace.
2. Status bar displays a Copilot icon.
3. Authenticate and sign in using a GitHub account with an active Copilot subscription (Free, Pro, Business, or Enterprise).

### B. Configuration & Customization
*   **Enable/Disable completions globally or per-language:**
    1. Click the Copilot status icon in the bottom status bar.
    2. Select **Disable completions** (global) or **Disable completions for [Language]**.
*   **Toggle Inline Suggestions:**
    1. Go to `Settings` (`Ctrl + ,` / `Cmd + ,`).
    2. Navigate to `Extensions` > `GitHub Copilot`.
    3. Toggle the checkbox for **Editor: Enable Auto Completions**.

### C. Troubleshooting Logs & Connection
When encountering network, firewall, or proxy errors, use these diagnostic pathways:

1.  **Diagnostic Report:**
    *   Open Command Palette (`Ctrl+Shift+P` / `Cmd+Shift+P`).
    *   Run **GitHub Copilot: Collect Diagnostics** to generate troubleshooting metadata.
2.  **Extension Logs:**
    *   Command Palette -> **Developer: Open Log File...** -> Choose **GitHub Copilot** log.
    *   Or **Developer: Open Extensions Logs Folder** to inspect historical files.
3.  **Electron/Core Logs:**
    *   Go to top menu -> **Help** > **Toggle Developer Tools** to inspect real-time console/network logs in the Electron developer console.
