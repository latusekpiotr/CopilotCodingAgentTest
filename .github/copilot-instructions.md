# CopilotCodingAgentTest Repository Instructions

**ALWAYS** follow these instructions first and only fallback to additional search and context gathering if the information in these instructions is incomplete or found to be in error.

## Repository Overview
CopilotCodingAgentTest is a minimal test repository containing only basic documentation. This repository serves as a testing ground for validating GitHub Copilot coding agent instructions and workflows.

## Working Effectively

### Repository Structure
The repository contains the following files and directories:
- `README.md` - Basic repository description
- `.github/` - GitHub configuration directory
- `.github/copilot-instructions.md` - This instructions file

### Basic Navigation and Git Operations
- Always start by checking repository status: `git status`
- View recent commits: `git log --oneline -10`
- Check current branch: `git branch -a`
- List repository contents: `ls -la`
- Find all markdown files: `find . -type f -name "*.md"`

### Build and Test Status
**IMPORTANT**: This repository contains NO build system, dependencies, or executable code.
- Do NOT attempt to run `npm install`, `yarn install`, or similar package management commands - no package.json exists
- Do NOT attempt to run build commands like `make`, `npm run build`, or similar - no build system exists
- Do NOT attempt to run test commands - no test framework exists
- Do NOT attempt to start servers or applications - no executable code exists

### Validation Steps
When working in this repository:
1. Always run `git status` to check for uncommitted changes
2. Always run `ls -la` to see current directory contents
3. Use `find . -type f -name "*" | grep -v ".git"` to see all non-git files
4. When adding new files, verify they appear in `git status`
5. Before committing, always run `git diff` to review changes

### Repository Limitations
- **No Code Execution**: This repository contains only documentation files
- **No Build Process**: There are no build scripts, makefiles, or package managers
- **No Dependencies**: No external dependencies to install or manage
- **No Testing Framework**: No unit tests, integration tests, or test runners
- **No CI/CD**: No GitHub Actions workflows or continuous integration setup

## Common Tasks

### Adding New Files
1. Create the file: `touch filename.ext`
2. Verify creation: `ls -la`
3. Check git status: `git status`
4. Add to git: `git add filename.ext`
5. Commit: `git commit -m "Add filename.ext"`

### Viewing Repository Contents
```bash
# List all files
ls -la

# Find all files excluding git
find . -type f | grep -v ".git"

# Count total files
find . -type f | grep -v ".git" | wc -l
```

### Expected Command Outputs
The following are outputs from frequently run commands. Reference them instead of running bash commands unnecessarily.

#### Repository Root Listing
```
$ ls -la
total 20
drwxr-xr-x 4 runner docker 4096 [date] .
drwxr-xr-x 3 runner docker 4096 [date] ..
drwxr-xr-x 7 runner docker 4096 [date] .git
drwxr-xr-x 2 runner docker 4096 [date] .github
-rw-r--r-- 1 runner docker   24 [date] README.md
```

#### README.md Contents
```
$ cat README.md
# CopilotCodingAgentTest
```

#### Git Status (Clean State)
```
$ git status
On branch [branch-name]
Your branch is up to date with 'origin/[branch-name]'.

nothing to commit, working tree clean
```

## File Modification Guidelines
- Always use absolute paths when referencing files: `/home/runner/work/CopilotCodingAgentTest/CopilotCodingAgentTest/`
- When editing files, use `str_replace_editor` with the `view` command first to understand current content
- For new files, use `str_replace_editor` with the `create` command
- For modifications, use `str_replace_editor` with the `str_replace` command

## What NOT to Do
- Do NOT run package manager commands (`npm install`, `yarn install`, `pip install`) - no package files exist
- Do NOT attempt to build the project - no build system exists
- Do NOT attempt to run servers or applications - no executable code exists
- Do NOT attempt to run tests - no test framework exists
- Do NOT create build artifacts or dependencies directories
- Do NOT add complex build processes without explicit requirements

## Emergency Procedures
If you encounter unexpected errors:
1. Run `git status` to check repository state
2. Run `ls -la` to verify file structure
3. Run `find . -type f | grep -v ".git"` to see all tracked files
4. If files are missing, check if you're in the correct directory: `pwd`
5. Verify you're using absolute paths: `/home/runner/work/CopilotCodingAgentTest/CopilotCodingAgentTest/`

## Success Criteria
You are working effectively in this repository when:
- You can successfully navigate and list repository contents
- You can create and modify files as needed
- You use git commands to track changes
- You understand this is a documentation-only repository with no executable code
- You avoid attempting to run non-existent build or test commands