import { useState, useEffect, createContext, useContext } from "react";

// ─── Fonts & Global Styles ───────────────────────────────────────────────────
const fontLink = document.createElement("link");
fontLink.rel = "stylesheet";
fontLink.href = "https://fonts.googleapis.com/css2?family=DM+Serif+Display:ital@0;1&family=DM+Sans:wght@300;400;500;600&display=swap";
document.head.appendChild(fontLink);

const style = document.createElement("style");
style.textContent = `
  *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

  :root {
    --bg:        #07090F;
    --surface:   #0D1018;
    --border:    rgba(255,255,255,0.07);
    --text:      #E8ECF4;
    --muted:     #4A5568;
    --accent:    #4F8EF7;
    --accent2:   #7C3AED;
    --danger:    #EF4444;
    --success:   #10B981;
    --warn:      #F59E0B;
  }

  html, body { height: 100%; }
  body { font-family: 'DM Sans', sans-serif; background: var(--bg); color: var(--text); -webkit-font-smoothing: antialiased; }

  ::-webkit-scrollbar { width: 3px; }
  ::-webkit-scrollbar-track { background: transparent; }
  ::-webkit-scrollbar-thumb { background: #1E2433; border-radius: 99px; }

  @keyframes fadeUp   { from { opacity:0; transform:translateY(20px); } to { opacity:1; transform:translateY(0); } }
  @keyframes fadeIn   { from { opacity:0; } to { opacity:1; } }
  @keyframes spin     { to { transform: rotate(360deg); } }
  @keyframes floatA   { 0%,100% { transform: translateY(0px) rotate(0deg); } 50% { transform: translateY(-18px) rotate(3deg); } }
  @keyframes floatB   { 0%,100% { transform: translateY(0px) rotate(0deg); } 50% { transform: translateY(-12px) rotate(-2deg); } }
  @keyframes shimmer  { from { background-position: -600px 0; } to { background-position: 600px 0; } }
  @keyframes glow     { 0%,100% { opacity:0.5; } 50% { opacity:1; } }

  .fade-up  { animation: fadeUp  0.55s cubic-bezier(.22,1,.36,1) both; }
  .fade-in  { animation: fadeIn  0.4s ease both; }
  .spin     { animation: spin    0.75s linear infinite; }
  .float-a  { animation: floatA  7s ease-in-out infinite; }
  .float-b  { animation: floatB  9s ease-in-out infinite; }

  .d1 { animation-delay: 0.05s; }
  .d2 { animation-delay: 0.12s; }
  .d3 { animation-delay: 0.19s; }
  .d4 { animation-delay: 0.26s; }

  /* Glass card */
  .glass {
    background: rgba(255,255,255,0.028);
    border: 1px solid var(--border);
    backdrop-filter: blur(16px);
  }

  /* Input */
  .field {
    width: 100%;
    padding: 11px 14px;
    border-radius: 10px;
    border: 1px solid rgba(255,255,255,0.09);
    background: rgba(255,255,255,0.03);
    color: var(--text);
    font-family: 'DM Sans', sans-serif;
    font-size: 14px;
    transition: border-color .2s, box-shadow .2s, background .2s;
  }
  .field:focus {
    outline: none;
    border-color: var(--accent);
    background: rgba(79,142,247,0.06);
    box-shadow: 0 0 0 3px rgba(79,142,247,0.13);
  }
  .field::placeholder { color: #2D3748; }

  /* Button */
  .btn {
    display: inline-flex; align-items: center; justify-content: center; gap: 8px;
    padding: 12px 24px; border-radius: 10px; border: none; cursor: pointer;
    font-family: 'DM Sans', sans-serif; font-size: 14px; font-weight: 600;
    transition: all .2s;
  }
  .btn-accent {
    background: linear-gradient(135deg, #4F8EF7 0%, #2563EB 100%);
    color: #fff;
    box-shadow: 0 4px 18px rgba(79,142,247,.32);
  }
  .btn-accent:hover:not(:disabled) { transform: translateY(-1px); box-shadow: 0 8px 26px rgba(79,142,247,.42); }
  .btn-accent:active { transform: translateY(0); }
  .btn-accent:disabled { opacity: .55; cursor: not-allowed; }
  .btn-ghost {
    background: rgba(255,255,255,.04);
    color: var(--muted);
    border: 1px solid var(--border);
  }
  .btn-ghost:hover { background: rgba(255,255,255,.07); color: var(--text); }

  /* Nav item */
  .nav-btn {
    display: flex; align-items: center; gap: 10px;
    width: 100%; padding: 9px 12px; border-radius: 9px;
    border: none; cursor: pointer; background: transparent;
    color: #3D4A60; font-family: 'DM Sans', sans-serif;
    font-size: 13.5px; font-weight: 500; text-align: left;
    transition: all .18s; border-left: 2px solid transparent;
    margin-bottom: 1px;
  }
  .nav-btn:hover  { background: rgba(79,142,247,.07); color: #8BA8D4; }
  .nav-btn.on     { background: rgba(79,142,247,.11); color: #7AADFF; border-left-color: var(--accent); }

  /* Course card hover */
  .ccard { transition: transform .22s, box-shadow .22s; }
  .ccard:hover { transform: translateY(-3px); box-shadow: 0 14px 40px rgba(0,0,0,.45); }

  /* Stat card */
  .scard { transition: transform .18s; }
  .scard:hover { transform: translateY(-2px); }

  /* Tag */
  .tag {
    display: inline-block; padding: 2px 9px; border-radius: 99px;
    font-size: 10.5px; font-weight: 600; letter-spacing: .05em; text-transform: uppercase;
  }

  /* Danger zone */
  .danger-zone { border: 1px solid rgba(239,68,68,.18); background: rgba(239,68,68,.03); border-radius: 14px; padding: 22px; }
`;
document.head.appendChild(style);

// ─── API ──────────────────────────────────────────────────────────────────────
const API = "/api";
const req = async (method, path, body, token) => {
  const h = { "Content-Type": "application/json" };
  if (token) h["Authorization"] = `Bearer ${token}`;
  const r = await fetch(`${API}${path}`, { method, headers: h, body: body ? JSON.stringify(body) : undefined });
  const d = await r.json().catch(() => ({}));
  if (!r.ok) throw new Error(d?.errors || d?.message || "Xatolik yuz berdi");
  return d;
};
const api = {
  login:         dto  => req("POST", "/auth/login", dto),
  register:      dto  => req("POST", "/auth/register", dto),
  refresh:       tok  => req("POST", "/auth/refresh", { refreshToken: tok }),
  updateProfile: (dto,t) => req("PUT",  "/users", dto, t),
  deleteAccount: t    => req("DELETE","/users", null, t),
};

// ─── Auth Context ──────────────────────────────────────────────────────────
const Ctx = createContext(null);
const useAuth = () => useContext(Ctx);

function AuthProvider({ children }) {
  const [user,  setUser]  = useState(() => { try { return JSON.parse(sessionStorage.getItem("u")); } catch { return null; } });
  const [at,    setAt]    = useState(() => sessionStorage.getItem("at") || null);
  const [rt,    setRt]    = useState(() => sessionStorage.getItem("rt") || null);

  const save = d => {
    setUser(d.user); setAt(d.accessToken); setRt(d.refreshToken);
    sessionStorage.setItem("u",  JSON.stringify(d.user));
    sessionStorage.setItem("at", d.accessToken);
    sessionStorage.setItem("rt", d.refreshToken);
  };
  const logout = () => { setUser(null); setAt(null); setRt(null); sessionStorage.clear(); };

  return <Ctx.Provider value={{ user, at, rt, save, logout }}>{children}</Ctx.Provider>;
}

// ─── Data ────────────────────────────────────────────────────────────────────
const COURSES = [
  { id:1, title:"React & TypeScript Masterclass",    cat:"Frontend",     dur:"42 soat", students:1240, level:"O'rta",        color:"#4F8EF7", icon:"⚛️",  rating:4.8 },
  { id:2, title:"ASP.NET Core 10 — Professional API",cat:"Backend",      dur:"38 soat", students:890,  level:"Yuqori",       color:"#7C3AED", icon:"🔷",  rating:4.9 },
  { id:3, title:"PostgreSQL & Entity Framework",      cat:"Database",     dur:"24 soat", students:670,  level:"Boshlang'ich", color:"#06B6D4", icon:"🗄️",  rating:4.7 },
  { id:4, title:"Docker & Kubernetes DevOps",         cat:"DevOps",       dur:"30 soat", students:520,  level:"Yuqori",       color:"#F59E0B", icon:"🐳",  rating:4.6 },
  { id:5, title:"Clean Architecture Patterns",        cat:"Architecture", dur:"20 soat", students:780,  level:"O'rta",        color:"#10B981", icon:"🏗️",  rating:4.9 },
  { id:6, title:"JWT & Auth Security",                cat:"Security",     dur:"16 soat", students:430,  level:"O'rta",        color:"#EF4444", icon:"🔐",  rating:4.8 },
];

// ─── Micro components ─────────────────────────────────────────────────────────
const Spinner = () => (
  <span className="spin" style={{ display:"inline-block", width:16, height:16, border:"2px solid rgba(255,255,255,.22)", borderTopColor:"#fff", borderRadius:"50%" }} />
);

const Label = ({ children }) => (
  <label style={{ display:"block", fontSize:12.5, fontWeight:600, color:"#3D4A60", letterSpacing:".04em", textTransform:"uppercase", marginBottom:6 }}>{children}</label>
);

const Field = ({ label, ...p }) => (
  <div style={{ marginBottom:18 }}>
    {label && <Label>{label}</Label>}
    <input className="field" {...p} />
  </div>
);

const Toast = ({ type, msg }) => {
  const c = type === "ok"
    ? { bg:"rgba(16,185,129,.1)", border:"rgba(16,185,129,.3)", text:"#6EE7B7" }
    : { bg:"rgba(239,68,68,.1)",  border:"rgba(239,68,68,.3)",  text:"#FCA5A5" };
  return (
    <div style={{ background:c.bg, border:`1px solid ${c.border}`, borderRadius:10, padding:"10px 14px", marginBottom:16, fontSize:13, color:c.text }}>
      {msg}
    </div>
  );
};

// ─── AUTH PAGE — split layout ─────────────────────────────────────────────────
function AuthPage({ onDone }) {
  const [tab,  setTab]  = useState("login");
  const [form, setForm] = useState({ name:"", email:"", password:"" });
  const [busy, setBusy] = useState(false);
  const [err,  setErr]  = useState("");
  const { save } = useAuth();

  const set = k => e => setForm(f => ({ ...f, [k]: e.target.value }));

  const submit = async () => {
    setErr("");
    if (!form.email || !form.password) return setErr("Email va parol kiritish shart!");
    if (tab === "register" && !form.name) return setErr("Ism kiritish shart!");
    setBusy(true);
    try {
      const d = tab === "login"
        ? await api.login({ email: form.email, password: form.password })
        : await api.register({ fullName: form.name, email: form.email, password: form.password });
      save(d);
      onDone();
    } catch(e) { setErr(e.message); }
    finally { setBusy(false); }
  };

  const onKey = e => e.key === "Enter" && submit();

  return (
    <div style={{ minHeight:"100vh", display:"grid", gridTemplateColumns:"1fr 1fr", background:"var(--bg)" }}>

      {/* ── Left decorative panel ── */}
      <div style={{ position:"relative", overflow:"hidden", display:"flex", flexDirection:"column", justifyContent:"space-between", padding:"48px", background:"linear-gradient(145deg, #090D18 0%, #0B0F1C 100%)", borderRight:"1px solid var(--border)" }}>

        {/* Grid texture */}
        <div style={{ position:"absolute", inset:0, backgroundImage:"linear-gradient(rgba(79,142,247,.04) 1px, transparent 1px), linear-gradient(90deg, rgba(79,142,247,.04) 1px, transparent 1px)", backgroundSize:"40px 40px", pointerEvents:"none" }} />

        {/* Glow blobs */}
        <div style={{ position:"absolute", top:"15%", left:"10%", width:300, height:300, background:"radial-gradient(circle, rgba(79,142,247,.12) 0%, transparent 65%)", pointerEvents:"none", animation:"glow 4s ease-in-out infinite" }} />
        <div style={{ position:"absolute", bottom:"20%", right:"5%",  width:220, height:220, background:"radial-gradient(circle, rgba(124,58,237,.1) 0%, transparent 65%)",  pointerEvents:"none", animation:"glow 6s ease-in-out infinite 1s" }} />

        {/* Logo */}
        <div style={{ position:"relative" }}>
          <div style={{ display:"flex", alignItems:"center", gap:12 }}>
            <div style={{ width:40, height:40, borderRadius:12, background:"linear-gradient(135deg,#4F8EF7,#2563EB)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:18, boxShadow:"0 6px 20px rgba(79,142,247,.35)" }}>🎓</div>
            <span style={{ fontFamily:"'DM Serif Display', serif", fontSize:20, color:"#fff", letterSpacing:".01em" }}>StudentCourse</span>
          </div>
        </div>

        {/* Center content */}
        <div style={{ position:"relative" }}>
          {/* Floating cards */}
          <div className="float-a glass" style={{ borderRadius:16, padding:"18px 22px", marginBottom:16, maxWidth:320 }}>
            <div style={{ display:"flex", alignItems:"center", gap:12 }}>
              <div style={{ width:40, height:40, borderRadius:10, background:"rgba(79,142,247,.15)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:18 }}>⚛️</div>
              <div>
                <p style={{ fontSize:13, fontWeight:600, color:"#C4D4EC" }}>React Masterclass</p>
                <div style={{ height:4, background:"rgba(255,255,255,.06)", borderRadius:99, marginTop:6, width:140 }}>
                  <div style={{ width:"72%", height:"100%", background:"linear-gradient(90deg,#4F8EF7,#7C3AED)", borderRadius:99 }} />
                </div>
              </div>
              <span style={{ marginLeft:"auto", fontSize:12, color:"#4F8EF7", fontWeight:600 }}>72%</span>
            </div>
          </div>

          <div className="float-b glass" style={{ borderRadius:16, padding:"18px 22px", maxWidth:280, marginLeft:40 }}>
            <div style={{ display:"flex", alignItems:"center", gap:12 }}>
              <div style={{ width:40, height:40, borderRadius:10, background:"rgba(16,185,129,.12)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:18 }}>🏆</div>
              <div>
                <p style={{ fontSize:13, fontWeight:600, color:"#C4D4EC" }}>Yangi sertifikat!</p>
                <p style={{ fontSize:11, color:"#3D4A60", marginTop:2 }}>Clean Architecture</p>
              </div>
            </div>
          </div>
        </div>

        {/* Bottom tagline */}
        <div style={{ position:"relative" }}>
          <h2 style={{ fontFamily:"'DM Serif Display', serif", fontSize:32, lineHeight:1.25, color:"#fff", marginBottom:12 }}>
            Professional<br />
            <em style={{ color:"#4F8EF7" }}>ta'lim</em> platformasi
          </h2>
          <p style={{ fontSize:14, color:"#2D3748", lineHeight:1.6 }}>
            Zamonaviy texnologiyalar bo'yicha<br />
            chuqur bilim oling
          </p>
        </div>
      </div>

      {/* ── Right form panel ── */}
      <div style={{ display:"flex", alignItems:"center", justifyContent:"center", padding:"48px 40px" }}>
        <div style={{ width:"100%", maxWidth:400 }} className="fade-up">

          {/* Heading */}
          <div style={{ marginBottom:36 }}>
            <h1 style={{ fontFamily:"'DM Serif Display', serif", fontSize:30, color:"#fff", marginBottom:6 }}>
              {tab === "login" ? "Xush kelibsiz" : "Ro'yxatdan o'ting"}
            </h1>
            <p style={{ fontSize:14, color:"var(--muted)" }}>
              {tab === "login" ? "Hisobingizga kiring" : "Yangi hisob yarating"}
            </p>
          </div>

          {/* Tab switcher */}
          <div style={{ display:"flex", gap:0, background:"rgba(255,255,255,.03)", border:"1px solid var(--border)", borderRadius:10, padding:4, marginBottom:28 }}>
            {["login","register"].map(t => (
              <button key={t} onClick={() => { setTab(t); setErr(""); }}
                style={{ flex:1, padding:"9px", border:"none", borderRadius:7, cursor:"pointer", fontFamily:"'DM Sans',sans-serif", fontSize:13.5, fontWeight:500, transition:"all .18s",
                  background: tab===t ? "rgba(79,142,247,.18)" : "transparent",
                  color:       tab===t ? "#7AADFF" : "var(--muted)"
                }}>
                {t === "login" ? "Kirish" : "Ro'yxat"}
              </button>
            ))}
          </div>

          {err && <Toast type="err" msg={err} />}

          {tab === "register" && (
            <Field label="To'liq ism" type="text" value={form.name} onChange={set("name")} placeholder="Ism Familiya" onKeyDown={onKey} />
          )}
          <Field label="Email" type="email" value={form.email} onChange={set("email")} placeholder="email@example.com" onKeyDown={onKey} />
          <Field label="Parol"  type="password" value={form.password} onChange={set("password")} placeholder="••••••••" onKeyDown={onKey} />

          <button className="btn btn-accent" onClick={submit} disabled={busy} style={{ width:"100%", padding:"13px", fontSize:15, marginTop:4, borderRadius:11 }}>
            {busy ? <><Spinner /> Yuklanmoqda...</> : (tab === "login" ? "Kirish →" : "Hisob yaratish →")}
          </button>

          <p style={{ marginTop:28, fontSize:12, color:"#1E2A3A", textAlign:"center" }}>
            Backend: localhost:7222
          </p>
        </div>
      </div>
    </div>
  );
}

// ─── Sidebar ──────────────────────────────────────────────────────────────────
function Sidebar({ page, set }) {
  const { user, logout } = useAuth();
  const nav = [
    { id:"dashboard", icon:"▦",  label:"Dashboard" },
    { id:"courses",   icon:"📚", label:"Kurslar"   },
    { id:"profile",   icon:"👤", label:"Profil"    },
  ];

  return (
    <aside style={{ width:220, minHeight:"100vh", display:"flex", flexDirection:"column", borderRight:"1px solid var(--border)", background:"rgba(7,9,15,.8)", flexShrink:0 }}>
      {/* Logo */}
      <div style={{ padding:"22px 18px 18px", borderBottom:"1px solid var(--border)" }}>
        <div style={{ display:"flex", alignItems:"center", gap:9 }}>
          <div style={{ width:32, height:32, borderRadius:9, background:"linear-gradient(135deg,#4F8EF7,#2563EB)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:14 }}>🎓</div>
          <span style={{ fontFamily:"'DM Serif Display',serif", fontSize:15, color:"#C8D6EC" }}>StudentCourse</span>
        </div>
      </div>

      {/* Nav */}
      <nav style={{ padding:"14px 10px", flex:1 }}>
        <p style={{ fontSize:10, fontWeight:700, color:"#1A2235", letterSpacing:".1em", textTransform:"uppercase", padding:"0 8px", marginBottom:8 }}>Menyu</p>
        {nav.map(n => (
          <button key={n.id} onClick={() => set(n.id)} className={`nav-btn ${page===n.id?"on":""}`}>
            <span style={{ fontSize:15 }}>{n.icon}</span>
            {n.label}
          </button>
        ))}
      </nav>

      {/* User */}
      <div style={{ padding:"14px", borderTop:"1px solid var(--border)" }}>
        <div style={{ display:"flex", alignItems:"center", gap:9, marginBottom:12 }}>
          <div style={{ width:32, height:32, borderRadius:8, background:"linear-gradient(135deg,#4F8EF7,#7C3AED)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:12, fontWeight:700, color:"#fff", flexShrink:0 }}>
            {user?.fullName?.[0] || "U"}
          </div>
          <div style={{ overflow:"hidden" }}>
            <p style={{ fontSize:12.5, fontWeight:600, color:"#8BA8D4", whiteSpace:"nowrap", overflow:"hidden", textOverflow:"ellipsis" }}>{user?.fullName || "Foydalanuvchi"}</p>
            <p style={{ fontSize:11, color:"#1E2A3A" }}>{user?.role || "Student"}</p>
          </div>
        </div>
        <button className="btn btn-ghost" onClick={logout} style={{ width:"100%", padding:"8px", fontSize:12.5, borderRadius:8, color:"rgba(239,68,68,.7)", borderColor:"rgba(239,68,68,.15)", background:"rgba(239,68,68,.05)" }}>
          Chiqish
        </button>
      </div>
    </aside>
  );
}

// ─── Dashboard ────────────────────────────────────────────────────────────────
function Dashboard() {
  const { user } = useAuth();
  const stats = [
    { label:"Faol kurslar",  val:"4",  icon:"📚", color:"#4F8EF7" },
    { label:"Tugallangan",   val:"2",  icon:"✅", color:"#10B981" },
    { label:"Sertifikatlar", val:"2",  icon:"🏆", color:"#F59E0B" },
    { label:"Soatlar",       val:"86", icon:"⏱️", color:"#7C3AED" },
  ];
  const activity = [
    { t:"React kursini tugatdingiz",     time:"2 soat oldin",  icon:"✅", c:"#10B981" },
    { t:"Yangi sertifikat oldingiz",      time:"Kecha",          icon:"🏆", c:"#F59E0B" },
    { t:"ASP.NET kursiga yozildingiz",   time:"3 kun oldin",    icon:"📚", c:"#4F8EF7" },
    { t:"Profilingizni yangiladingiz",   time:"1 hafta oldin",  icon:"👤", c:"#7C3AED" },
  ];

  return (
    <div style={{ padding:"36px 40px", maxWidth:1100, margin:"0 auto" }}>
      {/* Header */}
      <div className="fade-up" style={{ marginBottom:32 }}>
        <p style={{ fontSize:12, color:"var(--muted)", marginBottom:4, letterSpacing:".05em", textTransform:"uppercase" }}>Xush kelibsiz 👋</p>
        <h1 style={{ fontFamily:"'DM Serif Display',serif", fontSize:32, color:"#fff" }}>{user?.fullName || "Foydalanuvchi"}</h1>
      </div>

      {/* Stats */}
      <div className="fade-up d1" style={{ display:"grid", gridTemplateColumns:"repeat(4,1fr)", gap:14, marginBottom:28 }}>
        {stats.map((s,i) => (
          <div key={i} className="glass scard" style={{ borderRadius:14, padding:"20px 22px" }}>
            <div style={{ display:"flex", justifyContent:"space-between", alignItems:"flex-start" }}>
              <div>
                <p style={{ fontSize:11.5, color:"var(--muted)", marginBottom:10, fontWeight:500 }}>{s.label}</p>
                <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:34, color:"#fff" }}>{s.val}</p>
              </div>
              <div style={{ width:40, height:40, borderRadius:10, background:`${s.color}18`, display:"flex", alignItems:"center", justifyContent:"center", fontSize:18 }}>{s.icon}</div>
            </div>
          </div>
        ))}
      </div>

      {/* Bottom row */}
      <div className="fade-up d2" style={{ display:"grid", gridTemplateColumns:"1.4fr 1fr", gap:18 }}>
        {/* Recent courses */}
        <div className="glass" style={{ borderRadius:14, padding:"24px" }}>
          <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:17, color:"#C8D6EC", marginBottom:20 }}>Oxirgi kurslar</p>
          {COURSES.slice(0,4).map(c => (
            <div key={c.id} style={{ display:"flex", alignItems:"center", gap:12, marginBottom:16 }}>
              <div style={{ width:38, height:38, borderRadius:9, background:`${c.color}18`, display:"flex", alignItems:"center", justifyContent:"center", fontSize:17, flexShrink:0 }}>{c.icon}</div>
              <div style={{ flex:1, minWidth:0 }}>
                <p style={{ fontSize:12.5, fontWeight:500, color:"#8BA8D4", whiteSpace:"nowrap", overflow:"hidden", textOverflow:"ellipsis" }}>{c.title}</p>
                <div style={{ height:3, background:"rgba(255,255,255,.05)", borderRadius:99, marginTop:6 }}>
                  <div style={{ width:`${Math.floor(Math.random()*55+20)}%`, height:"100%", background:`linear-gradient(90deg,${c.color},${c.color}88)`, borderRadius:99 }} />
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Activity */}
        <div className="glass" style={{ borderRadius:14, padding:"24px" }}>
          <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:17, color:"#C8D6EC", marginBottom:20 }}>Faollik</p>
          {activity.map((a,i) => (
            <div key={i} style={{ display:"flex", gap:11, marginBottom:16 }}>
              <div style={{ width:32, height:32, borderRadius:8, background:`${a.c}14`, display:"flex", alignItems:"center", justifyContent:"center", fontSize:14, flexShrink:0 }}>{a.icon}</div>
              <div>
                <p style={{ fontSize:12.5, color:"#6A7F9A" }}>{a.t}</p>
                <p style={{ fontSize:11, color:"#1E2A3A", marginTop:2 }}>{a.time}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ─── Courses ──────────────────────────────────────────────────────────────────
function Courses() {
  const [q,   setQ]   = useState("");
  const [cat, setCat] = useState("Hammasi");
  const cats = ["Hammasi","Frontend","Backend","Database","DevOps","Architecture","Security"];
  const lvlColor = { "Boshlang'ich":"#10B981", "O'rta":"#F59E0B", "Yuqori":"#EF4444" };

  const list = COURSES.filter(c =>
    c.title.toLowerCase().includes(q.toLowerCase()) &&
    (cat === "Hammasi" || c.cat === cat)
  );

  return (
    <div style={{ padding:"36px 40px", maxWidth:1100, margin:"0 auto" }}>
      <div className="fade-up" style={{ marginBottom:28 }}>
        <h1 style={{ fontFamily:"'DM Serif Display',serif", fontSize:32, color:"#fff", marginBottom:4 }}>Kurslar</h1>
        <p style={{ fontSize:13, color:"var(--muted)" }}>{COURSES.length} ta kurs mavjud</p>
      </div>

      {/* Search + filter */}
      <div className="fade-up d1" style={{ display:"flex", gap:12, marginBottom:24, flexWrap:"wrap", alignItems:"center" }}>
        <input value={q} onChange={e=>setQ(e.target.value)} placeholder="🔍  Kurs qidirish..." className="field"
          style={{ width:260, padding:"10px 14px", borderRadius:10 }} />
        <div style={{ display:"flex", gap:6, flexWrap:"wrap" }}>
          {cats.map(c => (
            <button key={c} onClick={()=>setCat(c)}
              style={{ padding:"7px 13px", borderRadius:8, border:"1px solid", cursor:"pointer", fontFamily:"'DM Sans',sans-serif", fontSize:12.5, transition:"all .15s",
                borderColor: cat===c ? "rgba(79,142,247,.5)" : "var(--border)",
                background:  cat===c ? "rgba(79,142,247,.12)" : "rgba(255,255,255,.02)",
                color:       cat===c ? "#7AADFF" : "var(--muted)"
              }}>{c}</button>
          ))}
        </div>
      </div>

      <div className="fade-up d2" style={{ display:"grid", gridTemplateColumns:"repeat(auto-fill,minmax(290px,1fr))", gap:16 }}>
        {list.map(c => (
          <div key={c.id} className="glass ccard" style={{ borderRadius:14, overflow:"hidden", cursor:"pointer" }}>
            <div style={{ height:3, background:`linear-gradient(90deg,${c.color},${c.color}66)` }} />
            <div style={{ padding:20 }}>
              <div style={{ display:"flex", justifyContent:"space-between", alignItems:"flex-start", marginBottom:14 }}>
                <div style={{ width:44, height:44, borderRadius:12, background:`${c.color}15`, display:"flex", alignItems:"center", justifyContent:"center", fontSize:20 }}>{c.icon}</div>
                <div style={{ display:"flex", flexDirection:"column", alignItems:"flex-end", gap:5 }}>
                  <span className="tag" style={{ background:`${lvlColor[c.level]}14`, color:lvlColor[c.level] }}>{c.level}</span>
                  <span style={{ fontSize:12, color:"#F59E0B" }}>⭐ {c.rating}</span>
                </div>
              </div>
              <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:15, color:"#C8D6EC", marginBottom:4, lineHeight:1.4 }}>{c.title}</p>
              <p style={{ fontSize:11.5, color:"#2A3A52", marginBottom:14 }}>{c.cat}</p>
              <div style={{ display:"flex", justifyContent:"space-between", paddingTop:12, borderTop:"1px solid var(--border)" }}>
                <span style={{ fontSize:11.5, color:"var(--muted)" }}>⏱ {c.dur}</span>
                <span style={{ fontSize:11.5, color:"var(--muted)" }}>👥 {c.students.toLocaleString()}</span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {list.length === 0 && (
        <div style={{ textAlign:"center", padding:"80px 0", color:"#1E2A3A" }}>
          <p style={{ fontSize:36, marginBottom:12 }}>🔍</p>
          <p style={{ fontSize:14 }}>Kurs topilmadi</p>
        </div>
      )}
    </div>
  );
}

// ─── Profile ──────────────────────────────────────────────────────────────────
function Profile() {
  const { user, at, logout } = useAuth();
  const [form, setForm] = useState({ fullName: user?.fullName||"", email: user?.email||"" });
  const [busy,    setBusy]    = useState(false);
  const [err,     setErr]     = useState("");
  const [ok,      setOk]      = useState("");
  const [confirm, setConfirm] = useState(false);

  const set = k => e => setForm(f => ({ ...f, [k]: e.target.value }));

  const save = async () => {
    setErr(""); setOk(""); setBusy(true);
    try { await api.updateProfile(form, at); setOk("Profil muvaffaqiyatli yangilandi!"); }
    catch(e) { setErr(e.message); }
    finally { setBusy(false); }
  };

  const del = async () => {
    setBusy(true);
    try { await api.deleteAccount(at); logout(); }
    catch(e) { setErr(e.message); setBusy(false); }
  };

  return (
    <div style={{ padding:"36px 40px", maxWidth:580, margin:"0 auto" }}>
      <div className="fade-up" style={{ marginBottom:28 }}>
        <h1 style={{ fontFamily:"'DM Serif Display',serif", fontSize:32, color:"#fff", marginBottom:4 }}>Profil</h1>
        <p style={{ fontSize:13, color:"var(--muted)" }}>Ma'lumotlaringizni boshqaring</p>
      </div>

      {/* Avatar card */}
      <div className="fade-up d1 glass" style={{ borderRadius:14, padding:"22px 24px", marginBottom:18, display:"flex", alignItems:"center", gap:18 }}>
        <div style={{ width:64, height:64, borderRadius:16, background:"linear-gradient(135deg,#4F8EF7,#7C3AED)", display:"flex", alignItems:"center", justifyContent:"center", fontSize:24, fontWeight:700, color:"#fff", flexShrink:0 }}>
          {user?.fullName?.[0] || "U"}
        </div>
        <div>
          <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:20, color:"#C8D6EC" }}>{user?.fullName}</p>
          <p style={{ fontSize:13, color:"var(--muted)", marginTop:2 }}>{user?.email}</p>
          <span className="tag" style={{ background:"rgba(79,142,247,.12)", color:"#7AADFF", marginTop:8 }}>{user?.role || "Student"}</span>
        </div>
      </div>

      {/* Edit form */}
      <div className="fade-up d2 glass" style={{ borderRadius:14, padding:"24px", marginBottom:18 }}>
        <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:17, color:"#C8D6EC", marginBottom:20 }}>Ma'lumotlarni tahrirlash</p>
        {err && <Toast type="err" msg={err} />}
        {ok  && <Toast type="ok"  msg={ok}  />}
        <Field label="To'liq ism" type="text" value={form.fullName} onChange={set("fullName")} placeholder="Ism Familiya" />
        <Field label="Email" type="email" value={form.email} onChange={set("email")} placeholder="email@example.com" />
        <button className="btn btn-accent" onClick={save} disabled={busy} style={{ borderRadius:10 }}>
          {busy ? <><Spinner /> Saqlanmoqda...</> : "Saqlash"}
        </button>
      </div>

      {/* Danger */}
      <div className="fade-up d3 danger-zone">
        <p style={{ fontFamily:"'DM Serif Display',serif", fontSize:16, color:"rgba(239,68,68,.8)", marginBottom:6 }}>Xavfli zona</p>
        <p style={{ fontSize:13, color:"#2A3A52", marginBottom:16 }}>Hisobingizni o'chirsangiz, barcha ma'lumotlar yo'qoladi.</p>
        {!confirm
          ? <button className="btn btn-ghost" onClick={()=>setConfirm(true)} style={{ fontSize:13, color:"rgba(239,68,68,.65)", borderColor:"rgba(239,68,68,.2)" }}>Hisobni o'chirish</button>
          : <div style={{ display:"flex", gap:10, alignItems:"center" }}>
              <p style={{ fontSize:13, color:"rgba(239,68,68,.7)" }}>Ishonchingiz komilmi?</p>
              <button className="btn" onClick={del} disabled={busy}
                style={{ background:"#EF4444", color:"#fff", padding:"8px 16px", fontSize:13, borderRadius:9 }}>
                {busy ? "..." : "Ha, o'chir"}
              </button>
              <button className="btn btn-ghost" onClick={()=>setConfirm(false)} style={{ padding:"8px 16px", fontSize:13, borderRadius:9 }}>Bekor</button>
            </div>
        }
      </div>
    </div>
  );
}

// ─── Layout ───────────────────────────────────────────────────────────────────
function Layout() {
  const [page, setPage] = useState("dashboard");
  const views = { dashboard:<Dashboard/>, courses:<Courses/>, profile:<Profile/> };
  return (
    <div style={{ display:"flex", minHeight:"100vh" }}>
      <Sidebar page={page} set={setPage} />
      <main style={{ flex:1, overflowY:"auto" }}>{views[page]}</main>
    </div>
  );
}

// ─── Root ─────────────────────────────────────────────────────────────────────
function App() {
  const [authed, setAuthed] = useState(false);
  const { user } = useAuth();
  useEffect(() => { if (user) setAuthed(true); }, []);
  return authed ? <Layout /> : <AuthPage onDone={() => setAuthed(true)} />;
}

export default function Root() {
  return <AuthProvider><App /></AuthProvider>;
}
