# 🎮 Lost Orbit – Unity Game Project  
**Unity Editor Version:** 6000.2.7f2  

---

## 📁 Project Structure

Assets/ → Game scenes, scripts, prefabs, materials

Packages/ → Unity packages

ProjectSettings/ → Engine and build configuration

.gitignore → Prevents unnecessary Unity files (Library, Logs, Temp)


---

## 🧩 Setup Instructions

### 1️⃣ Clone the Repository
Use **Git Bash**, **Command Prompt**, or **GitHub Desktop** to clone the project:

git clone https://github.com/kasiaskrz/Y4_LostOrbit_Game


Then open **Unity Hub → Projects → Add project from disk →** select this folder.

---

### 2️⃣ Unity Version
All team members must use the same Unity Editor version:  
> **6000.2.7f2**

Otherwise, you may get errors when opening scenes or prefabs.

---

## ⚙️ Working as a Team

You’ll use **Git commands** to send and receive updates between your computer and GitHub.  
These commands are typed in a **Command Prompt**, **PowerShell**, or **Git Bash** window inside your Unity project folder.

---

### 🖥️ Where to Type the Commands

1️⃣ Open your Unity project folder in File Explorer (for example):  
C:\UnityProjects\LostOrbit_Game


2️⃣ Right-click inside the folder (not on a file), then choose:  
> “Open in Terminal”, “Open PowerShell window here”, or “Git Bash here”

3️⃣ The prompt should look like this:  
C:\UnityProjects\LostOrbit_Game>


That means you’re in the right place to type Git commands.

---

### 💾 Common Commands

**Before you start working (to get the newest updates):**

git pull


**After you make changes in Unity (to save and upload your work):**

git add .

git commit -m "Describe your change (e.g. Added new puzzle script)"

git push


**If you get updates from others:**

git pull


---

### 💡 Tips

Check what files you’ve changed:

git status


If you ever see a “push rejected” message (someone else pushed first):

git pull --rebase

git push


---

## 🏁 Notes

- Don’t edit the same scene file as another team member at the same time.  
- Use your own branch for large changes:
- 
git checkout -b feature/puzzle-system

- Always commit **small, clear updates** — not huge dumps of work.  
- Always **pull before starting** and **push when finished**.  
