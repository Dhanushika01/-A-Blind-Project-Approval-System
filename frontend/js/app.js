const API_BASE = 'http://localhost:5000/api';
let token = localStorage.getItem('token') || '';

const loginOverlay = document.getElementById('login-overlay');
const loginBtn = document.getElementById('login-btn');
const logoutBtn = document.getElementById('logout-btn');
const navLinks = document.querySelectorAll('.nav-link');
const views = document.querySelectorAll('.view');

if (token) {
    loginOverlay.classList.remove('active');
    loadDashboard();
}

navLinks.forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();
        if (link.classList.contains('disabled')) return;

        navLinks.forEach(l => l.classList.remove('active'));
        link.classList.add('active');

        const targetView = link.getAttribute('data-view');
        views.forEach(view => {
            if (view.id === `${targetView}-view`) {
                view.classList.add('active');
                view.classList.remove('hidden');
            } else {
                view.classList.remove('active');
                view.classList.add('hidden');
            }
        });

        if (targetView === 'dashboard') loadDashboard();
        if (targetView === 'projects-list') loadProjectsList();
    });
});

loginBtn.addEventListener('click', async () => {
    const user = document.getElementById('login-username').value;
    const pass = document.getElementById('login-password').value;
    const err = document.getElementById('login-error');

    if (!user || !pass) { err.textContent = 'Please enter both fields'; return; }

    try {
        loginBtn.textContent = 'Logging in...';
        let res = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: user, password: pass })
        });

        if (!res.ok) {
            await fetch(`${API_BASE}/auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: user, name: user.split('@')[0], password: pass, role: 'student' })
            });
            res = await fetch(`${API_BASE}/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: user, password: pass })
            });
        }

        if (res.ok) {
            const data = await res.json();
            token = data.token;
            localStorage.setItem('token', token);
            loginOverlay.classList.remove('active');
            loadDashboard();
        } else {
            err.textContent = 'Login failed. Please check backend is running.';
        }
    } catch (e) {
        err.textContent = 'Cannot connect to server. Is it running?';
        console.error(e);
    } finally {
        loginBtn.textContent = 'Login / Register';
    }
});

logoutBtn.addEventListener('click', () => {
    localStorage.removeItem('token');
    token = '';
    loginOverlay.classList.add('active');
});

async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    };
    if (body) options.body = JSON.stringify(body);

    const res = await fetch(`${API_BASE}${endpoint}`, options);
    if (res.status === 401) {
        logoutBtn.click();
        throw new Error('Unauthorized');
    }
    if (!res.ok) throw new Error('API Error');
    if (method === 'DELETE') return true;
    return await res.json();
}

async function loadDashboard() {
    const activityList = document.getElementById('activity-list');
    activityList.innerHTML = '<div class="activity-item loading">Loading...</div>';

    try {
        const projects = await apiCall('/projects/my');

        if (projects.length === 0) {
            activityList.innerHTML = '<div class="activity-item text-muted">No activity yet. Submit a project to get started!</div>';
            updateStats([]);
            return;
        }

        activityList.innerHTML = '';
        projects.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)).forEach(p => {
            const date = new Date(p.createdAt).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
            let icon = 'pulse-outline';
            let title = `New project "${p.title}" submitted`;

            if (p.status === 'matched') {
                icon = 'checkmark-circle-outline';
                title = `Project "${p.title}" matched with reviewer`;
            } else if (p.status === 'rejected') {
                icon = 'close-circle-outline';
                title = `Project "${p.title}" was rejected`;
            }

            activityList.innerHTML += `
                <div class="activity-item">
                    <div class="activity-icon"><ion-icon name="${icon}"></ion-icon></div>
                    <div class="activity-content">
                        <div class="activity-title">${title}</div>
                        <div class="activity-project">${p.title}</div>
                    </div>
                    <div class="activity-date">${date}</div>
                </div>
            `;
        });

        updateStats(projects);
    } catch (e) {
        activityList.innerHTML = '<div class="activity-item error-msg">Failed to load activity.</div>';
    }
}

function updateStats(projects) {
    const areas = { 'Infrastructure': { t: 0, a: 0 }, 'Technology': { t: 0, a: 0 }, 'Healthcare': { t: 0, a: 0 }, 'Energy': { t: 0, a: 0 }, 'Education': { t: 0, a: 0 } };

    projects.forEach(p => {
        if (areas[p.researchArea]) {
            areas[p.researchArea].t++;
            if (p.status === 'matched') areas[p.researchArea].a++;
        }
    });

    const statGroups = document.querySelectorAll('.stat-group');
    statGroups.forEach(group => {
        const title = group.querySelector('.stat-header span:first-child').textContent;
        const data = areas[title];
        if (data) {
            group.querySelector('.stat-header span:last-child').textContent = `${data.t} total`;
            group.querySelector('.stat-footer').textContent = `${data.a} matched`;
            const pct = data.t === 0 ? 0 : (data.a / data.t) * 100;
            group.querySelector('.fill').style.width = `${pct}%`;
        }
    });
}

async function loadProjectsList() {
    const grid = document.getElementById('my-projects-grid');
    grid.innerHTML = '<p>Loading projects...</p>';

    try {
        const projects = await apiCall('/projects/my');
        if (projects.length === 0) {
            grid.innerHTML = '<p class="text-muted">You haven\'t submitted any projects yet.</p>';
            return;
        }

        grid.innerHTML = '';
        projects.forEach(p => {
            const canEdit = p.status === 'pending';
            const buttons = canEdit
                ? `<button class="btn outline-btn edit-btn" data-id="${p.id}" data-project='${JSON.stringify(p).replace(/'/g, "&apos;")}'>
                     <ion-icon name="create-outline"></ion-icon> Edit
                   </button>
                   <button class="btn danger-btn delete-btn" data-id="${p.id}">
                     <ion-icon name="trash-outline"></ion-icon> Delete
                   </button>`
                : `<span class="text-muted">Cannot edit (Status: ${p.status})</span>`;

            grid.innerHTML += `
                <div class="project-card card">
                    <h3>${p.title}</h3>
                    <div class="project-meta">
                        <span class="badge ${p.status}">${p.status.toUpperCase()}</span>
                        <span class="badge" style="background:#e2e8f0; color:#334155">${p.researchArea}</span>
                    </div>
                    <p>${p.abstract.substring(0, 100)}...</p>
                    <div class="project-actions">
                        ${buttons}
                    </div>
                </div>
            `;
        });

        document.querySelectorAll('.edit-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const projectData = JSON.parse(e.currentTarget.getAttribute('data-project'));
                openEditView(projectData);
            });
        });

        document.querySelectorAll('.delete-btn').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                if (confirm('Are you sure you want to delete this proposal?')) {
                    const id = e.currentTarget.getAttribute('data-id');
                    try {
                        await apiCall(`/projects/my/${id}`, 'DELETE');
                        loadProjectsList();
                        loadDashboard();
                    } catch (err) {
                        alert('Failed to delete project');
                    }
                }
            });
        });

    } catch (e) {
        grid.innerHTML = '<p class="error-msg">Failed to load projects.</p>';
    }
}

document.getElementById('submit-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('submit-project-btn');
    btn.textContent = 'Submitting...';

    const body = {
        title: document.getElementById('title').value,
        researchArea: document.getElementById('researchArea').value,
        techStack: document.getElementById('techStack').value,
        abstract: document.getElementById('abstract').value
    };

    try {
        await apiCall('/projects', 'POST', body);
        document.getElementById('submit-form').reset();
        alert('Project submitted successfully!');
        navLinks[0].click();
    } catch (err) {
        alert('Failed to submit project');
    } finally {
        btn.textContent = 'Submit Proposal';
    }
});

function openEditView(project) {
    document.getElementById('edit-id').value = project.id;
    document.getElementById('edit-title').value = project.title;
    document.getElementById('edit-researchArea').value = project.researchArea;
    document.getElementById('edit-techStack').value = project.techStack;
    document.getElementById('edit-abstract').value = project.abstract;

    views.forEach(view => view.classList.add('hidden'));
    document.getElementById('edit-project-view').classList.remove('hidden');
    navLinks.forEach(l => l.classList.remove('active'));
}

document.getElementById('cancel-edit-btn').addEventListener('click', () => {
    navLinks[1].click();
});

document.getElementById('edit-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('edit-id').value;
    const btn = document.getElementById('save-edit-btn');
    btn.textContent = 'Saving...';

    const body = {
        title: document.getElementById('edit-title').value,
        researchArea: document.getElementById('edit-researchArea').value,
        techStack: document.getElementById('edit-techStack').value,
        abstract: document.getElementById('edit-abstract').value
    };

    try {
        await apiCall(`/projects/my/${id}`, 'PUT', body);
        alert('Project updated successfully!');
        navLinks[1].click();
    } catch (err) {
        alert('Failed to update project');
    } finally {
        btn.textContent = 'Save Changes';
    }
});
