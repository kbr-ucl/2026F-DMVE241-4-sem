import { useState } from "react";

const COLORS = {
  bg: "#0A0E17", surface: "#111827", surfaceHover: "#1a2235",
  card: "#151d2e", cardHover: "#1c2740",
  border: "#1e293b", borderLight: "#2d3a50",
  primary: "#f97316", primaryDark: "#ea580c", primaryGlow: "rgba(249,115,22,0.15)",
  accent: "#06b6d4", accentGlow: "rgba(6,182,212,0.12)",
  text: "#f1f5f9", textMuted: "#94a3b8", textDim: "#64748b",
  success: "#22c55e", danger: "#ef4444", warning: "#eab308",
  tagBg: "rgba(249,115,22,0.1)", tagBorder: "rgba(249,115,22,0.25)",
};

const initialEvents = [
  { id: 1, name: "Copenhagen Jazz Festival", date: "2026-07-04", location: "Tivoli Gardens, K\u00f8benhavn", description: "En aften med verdensklasse jazzmusikere i hjertet af K\u00f8benhavn.", status: "Published", categories: [{ id: 1, name: "Standard", price: 350, available: 120, total: 200 }, { id: 2, name: "VIP", price: 850, available: 8, total: 50 }] },
  { id: 2, name: "Tech Summit 2026", date: "2026-09-15", location: "Bella Center, K\u00f8benhavn", description: "Nordens st\u00f8rste teknologikonference med 200+ talks om AI, cloud og microservices.", status: "Published", categories: [{ id: 3, name: "Early Bird", price: 1200, available: 0, total: 100 }, { id: 4, name: "Standard", price: 1800, available: 340, total: 500 }, { id: 5, name: "VIP", price: 4500, available: 22, total: 30 }] },
  { id: 3, name: "Hamlet p\u00e5 Kronborg", date: "2026-08-20", location: "Kronborg Slot, Helsing\u00f8r", description: "Shakespeares klassiker opf\u00f8rt under \u00e5ben himmel p\u00e5 det ikoniske Kronborg Slot.", status: "Published", categories: [{ id: 6, name: "Tribune", price: 450, available: 85, total: 150 }, { id: 7, name: "Gulvplads", price: 280, available: 200, total: 300 }] },
  { id: 4, name: "Roskilde Festival Pre-Party", date: "2026-06-28", location: "Musicon, Roskilde", description: "Officiel opvarmning til Roskilde Festival med 6 upcoming danske bands.", status: "AlmostSoldOut", categories: [{ id: 8, name: "Entr\u00e9", price: 175, available: 12, total: 500 }] },
];

const initialOrders = [
  { id: "ORD-7841", eventName: "Copenhagen Jazz Festival", email: "kunde@mail.dk", status: "Confirmed", total: 2550, createdAt: "2026-02-17T14:32:00", lines: [{ category: "Standard", qty: 2, unitPrice: 350 }, { category: "VIP", qty: 2, unitPrice: 850 }] },
  { id: "ORD-7842", eventName: "Tech Summit 2026", email: "kunde@mail.dk", status: "Cancelled", total: 1800, createdAt: "2026-02-18T09:11:00", lines: [{ category: "Standard", qty: 1, unitPrice: 1800 }] },
  { id: "ORD-7843", eventName: "Hamlet p\u00e5 Kronborg", email: "kunde@mail.dk", status: "Created", total: 900, createdAt: "2026-02-18T10:45:00", lines: [{ category: "Tribune", qty: 2, unitPrice: 450 }] },
];

const StatusBadge = ({ status }) => {
  const map = {
    Confirmed: { bg: "rgba(34,197,94,0.12)", color: COLORS.success, border: "rgba(34,197,94,0.3)" },
    Cancelled: { bg: "rgba(239,68,68,0.12)", color: COLORS.danger, border: "rgba(239,68,68,0.3)" },
    Created: { bg: "rgba(234,179,8,0.12)", color: COLORS.warning, border: "rgba(234,179,8,0.3)" },
    Published: { bg: COLORS.tagBg, color: COLORS.primary, border: COLORS.tagBorder },
    Draft: { bg: "rgba(100,116,139,0.15)", color: COLORS.textDim, border: "rgba(100,116,139,0.3)" },
    AlmostSoldOut: { bg: "rgba(239,68,68,0.12)", color: COLORS.danger, border: "rgba(239,68,68,0.3)" },
  };
  const s = map[status] || map.Created;
  return (<span style={{ display: "inline-flex", alignItems: "center", gap: 5, padding: "3px 10px", borderRadius: 20, fontSize: 11, fontWeight: 600, letterSpacing: 0.5, textTransform: "uppercase", background: s.bg, color: s.color, border: `1px solid ${s.border}` }}><span style={{ width: 6, height: 6, borderRadius: "50%", background: s.color }} />{status === "AlmostSoldOut" ? "N\u00e6sten udsolgt" : status}</span>);
};

const AvailabilityBar = ({ available, total }) => {
  const pct = total > 0 ? (available / total) * 100 : 0;
  const color = pct < 10 ? COLORS.danger : pct < 30 ? COLORS.warning : COLORS.success;
  return (<div style={{ display: "flex", alignItems: "center", gap: 8 }}><div style={{ flex: 1, height: 4, borderRadius: 2, background: COLORS.border }}><div style={{ width: `${pct}%`, height: "100%", borderRadius: 2, background: color, transition: "width 0.6s ease" }} /></div><span style={{ fontSize: 11, color: COLORS.textDim, whiteSpace: "nowrap" }}>{available}/{total}</span></div>);
};

const Icon = ({ name, size = 16 }) => {
  const i = {
    calendar: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>,
    pin: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/><circle cx="12" cy="10" r="3"/></svg>,
    ticket: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z"/><path d="M13 5v2"/><path d="M13 17v2"/><path d="M13 11v2"/></svg>,
    cart: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><circle cx="8" cy="21" r="1"/><circle cx="19" cy="21" r="1"/><path d="M2.05 2.05h2l2.66 12.42a2 2 0 0 0 2 1.58h9.78a2 2 0 0 0 1.95-1.57l1.65-7.43H5.12"/></svg>,
    back: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><line x1="19" y1="12" x2="5" y2="12"/><polyline points="12 19 5 12 12 5"/></svg>,
    check: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><polyline points="20 6 9 17 4 12"/></svg>,
    health: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>,
    list: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/><line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/><line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/></svg>,
    gateway: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/></svg>,
    plus: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>,
    edit: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>,
    send: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/></svg>,
    trash: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>,
    chevron: <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round"><polyline points="9 18 15 12 9 6"/></svg>,
  };
  return i[name] || null;
};

const InputField = ({ label, type = "text", value, onChange, placeholder, error, textarea }) => (
  <div style={{ marginBottom: 16 }}>
    <label style={{ display: "block", fontSize: 12, fontWeight: 600, color: COLORS.textMuted, marginBottom: 6, textTransform: "uppercase", letterSpacing: 0.5 }}>{label}</label>
    {textarea ? (
      <textarea value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder} rows={3}
        style={{ width: "100%", padding: "10px 12px", borderRadius: 8, border: `1px solid ${error ? COLORS.danger : COLORS.border}`, background: COLORS.surface, color: COLORS.text, fontSize: 13, fontFamily: "inherit", resize: "vertical", outline: "none", boxSizing: "border-box" }} />
    ) : (
      <input type={type} value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder}
        style={{ width: "100%", padding: "10px 12px", borderRadius: 8, border: `1px solid ${error ? COLORS.danger : COLORS.border}`, background: COLORS.surface, color: COLORS.text, fontSize: 13, fontFamily: "inherit", outline: "none", boxSizing: "border-box" }} />
    )}
    {error && <div style={{ fontSize: 11, color: COLORS.danger, marginTop: 4 }}>{error}</div>}
  </div>
);

const Btn = ({ children, onClick, variant = "primary", disabled, icon, full, small }) => {
  const styles = {
    primary: { bg: `linear-gradient(135deg, ${COLORS.primary}, ${COLORS.primaryDark})`, color: "#fff", border: "none" },
    secondary: { bg: COLORS.surface, color: COLORS.text, border: `1px solid ${COLORS.border}` },
    success: { bg: "rgba(34,197,94,0.12)", color: COLORS.success, border: `1px solid rgba(34,197,94,0.25)` },
  };
  const s = styles[variant] || styles.primary;
  return (
    <button disabled={disabled} onClick={onClick} style={{ display: "inline-flex", alignItems: "center", justifyContent: "center", gap: 7, padding: small ? "6px 12px" : "10px 20px", borderRadius: small ? 6 : 10, background: disabled ? COLORS.border : s.bg, color: disabled ? COLORS.textDim : s.color, border: s.border, fontSize: small ? 12 : 13, fontWeight: 600, cursor: disabled ? "not-allowed" : "pointer", fontFamily: "inherit", width: full ? "100%" : undefined, transition: "all 0.2s", opacity: disabled ? 0.6 : 1 }}
      onMouseEnter={e => { if (!disabled) e.currentTarget.style.opacity = "0.85"; }}
      onMouseLeave={e => { e.currentTarget.style.opacity = disabled ? "0.6" : "1"; }}>
      {icon && <Icon name={icon} size={small ? 13 : 15} />}{children}
    </button>
  );
};

const ApiFlowBox = ({ label, text }) => (
  <div style={{ marginTop: 16, padding: 12, borderRadius: 8, background: COLORS.accentGlow, border: `1px solid rgba(6,182,212,0.15)` }}>
    <div style={{ fontSize: 10, fontWeight: 600, color: COLORS.accent, textTransform: "uppercase", letterSpacing: 0.5, marginBottom: 4 }}>{label || "API-flow"}</div>
    <div style={{ fontSize: 11, color: COLORS.textDim, lineHeight: 1.5 }}>{text}</div>
  </div>
);

const BackBtn = ({ onClick, label }) => (
  <button onClick={onClick} style={{ display: "flex", alignItems: "center", gap: 6, background: "none", border: "none", color: COLORS.textMuted, fontSize: 13, cursor: "pointer", padding: 0, marginBottom: 24, fontFamily: "inherit" }}>
    <Icon name="back" size={14} /> {label}
  </button>
);

const navItems = [
  { id: "events", label: "Events", icon: "ticket" },
  { id: "manage", label: "Arrang\u00f8r", icon: "edit" },
  { id: "orders", label: "Ordrer", icon: "list" },
  { id: "health", label: "Sundhed", icon: "health" },
];

export default function TicketHubUI() {
  const [page, setPage] = useState("events");
  const [events, setEvents] = useState(initialEvents);
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [orderCart, setOrderCart] = useState(null);
  const [orderResult, setOrderResult] = useState(null);
  const [quantities, setQuantities] = useState({});
  const [nextId, setNextId] = useState(100);
  const [editingEvent, setEditingEvent] = useState(null);
  const [newCat, setNewCat] = useState({ name: "", price: "", total: "" });
  const [catErrors, setCatErrors] = useState({});
  const [eventForm, setEventForm] = useState({ name: "", date: "", location: "", description: "" });
  const [eventErrors, setEventErrors] = useState({});
  const [toast, setToast] = useState(null);

  const showToast = (msg, type = "success") => { setToast({ msg, type }); setTimeout(() => setToast(null), 3500); };
  const isActive = (id) => page === id || (id === "events" && ["eventDetail", "order"].includes(page)) || (id === "manage" && ["manageEvent", "createEvent"].includes(page));

  const navigateTo = (p, data) => {
    setPage(p);
    if (p === "eventDetail") setSelectedEvent(data);
    if (p === "order") { setOrderCart(data); setQuantities({}); setOrderResult(null); }
    if (p === "events") { setSelectedEvent(null); setOrderCart(null); setOrderResult(null); }
    if (p === "createEvent") { setEventForm({ name: "", date: "", location: "", description: "" }); setEventErrors({}); }
    if (p === "manageEvent") { setEditingEvent(data); setNewCat({ name: "", price: "", total: "" }); setCatErrors({}); }
  };

  const handleCreateEvent = () => {
    const errs = {};
    if (!eventForm.name.trim()) errs.name = "Navn er p\u00e5kr\u00e6vet";
    if (!eventForm.date) errs.date = "Dato er p\u00e5kr\u00e6vet";
    else if (new Date(eventForm.date) <= new Date()) errs.date = "Dato skal v\u00e6re i fremtiden";
    if (!eventForm.location.trim()) errs.location = "Sted er p\u00e5kr\u00e6vet";
    setEventErrors(errs);
    if (Object.keys(errs).length > 0) return;
    const id = nextId; setNextId(n => n + 1);
    const ev = { id, ...eventForm, status: "Draft", categories: [] };
    setEvents(prev => [ev, ...prev]);
    showToast(`Event "${ev.name}" oprettet med status Draft`);
    navigateTo("manageEvent", ev);
  };

  const handleAddCategory = () => {
    const errs = {};
    if (!newCat.name.trim()) errs.name = "P\u00e5kr\u00e6vet";
    const price = parseFloat(newCat.price);
    if (isNaN(price) || price < 0) errs.price = "\u2265 0";
    const total = parseInt(newCat.total);
    if (isNaN(total) || total <= 0) errs.total = "> 0";
    setCatErrors(errs);
    if (Object.keys(errs).length > 0) return;
    const catId = nextId; setNextId(n => n + 1);
    const cat = { id: catId, name: newCat.name.trim(), price, available: total, total };
    setEvents(prev => prev.map(e => e.id === editingEvent.id ? { ...e, categories: [...e.categories, cat] } : e));
    setEditingEvent(prev => ({ ...prev, categories: [...prev.categories, cat] }));
    setNewCat({ name: "", price: "", total: "" }); setCatErrors({});
    showToast(`Kategori "${cat.name}" tilf\u00f8jet`);
  };

  const handleRemoveCategory = (catId) => {
    setEvents(prev => prev.map(e => e.id === editingEvent.id ? { ...e, categories: e.categories.filter(c => c.id !== catId) } : e));
    setEditingEvent(prev => ({ ...prev, categories: prev.categories.filter(c => c.id !== catId) }));
  };

  const handlePublishEvent = () => {
    if (editingEvent.categories.length === 0) { showToast("Tilf\u00f8j mindst \u00e9n billetkategori f\u00f8r publicering", "error"); return; }
    setEvents(prev => prev.map(e => e.id === editingEvent.id ? { ...e, status: "Published" } : e));
    setEditingEvent(prev => ({ ...prev, status: "Published" }));
    showToast(`"${editingEvent.name}" publiceret! EventCreated \u2192 Inventory`);
  };

  const handleOrder = () => {
    const lines = Object.entries(quantities).filter(([, q]) => q > 0).map(([catId, qty]) => {
      const cat = orderCart.categories.find(c => c.id === parseInt(catId));
      return { category: cat.name, qty, unitPrice: cat.price };
    });
    if (lines.length === 0) return;
    const total = lines.reduce((s, l) => s + l.qty * l.unitPrice, 0);
    setOrderResult({ id: `ORD-${Math.floor(Math.random() * 9000) + 1000}`, status: "Created", total, lines, eventName: orderCart.name });
  };

  const publishedEvents = events.filter(e => e.status === "Published" || e.status === "AlmostSoldOut");

  return (
    <div style={{ minHeight: "100vh", background: COLORS.bg, color: COLORS.text, fontFamily: "'DM Sans', 'Helvetica Neue', sans-serif" }}>
      <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,wght@0,400;0,500;0,600;0,700&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet" />

      {toast && <div style={{ position: "fixed", top: 68, left: "50%", transform: "translateX(-50%)", zIndex: 100, padding: "10px 20px", borderRadius: 10, background: toast.type === "error" ? "rgba(239,68,68,0.95)" : "rgba(34,197,94,0.95)", color: "#fff", fontSize: 13, fontWeight: 600, backdropFilter: "blur(10px)", animation: "fadeUp 0.3s ease", boxShadow: "0 8px 32px rgba(0,0,0,0.4)" }}>{toast.msg}</div>}

      <nav style={{ position: "sticky", top: 0, zIndex: 50, background: "rgba(10,14,23,0.85)", backdropFilter: "blur(20px)", borderBottom: `1px solid ${COLORS.border}` }}>
        <div style={{ maxWidth: 1100, margin: "0 auto", padding: "0 24px", display: "flex", alignItems: "center", height: 56 }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }} onClick={() => navigateTo("events")}>
            <div style={{ width: 28, height: 28, borderRadius: 6, background: `linear-gradient(135deg, ${COLORS.primary}, ${COLORS.primaryDark})`, display: "flex", alignItems: "center", justifyContent: "center" }}><Icon name="ticket" size={14} /></div>
            <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 15, letterSpacing: -0.5 }}>TicketHub</span>
          </div>
          <div style={{ flex: 1 }} />
          <div style={{ display: "flex", gap: 2 }}>
            {navItems.map(n => (
              <button key={n.id} onClick={() => navigateTo(n.id)} style={{ display: "flex", alignItems: "center", gap: 6, padding: "6px 14px", borderRadius: 8, border: "none", background: isActive(n.id) ? COLORS.primaryGlow : "transparent", color: isActive(n.id) ? COLORS.primary : COLORS.textDim, fontSize: 13, fontWeight: 500, cursor: "pointer", transition: "all 0.2s", fontFamily: "inherit" }}>
                <Icon name={n.icon} size={14} />{n.label}
              </button>
            ))}
          </div>
          <div style={{ marginLeft: 16, padding: "4px 10px", borderRadius: 6, background: COLORS.accentGlow, border: `1px solid rgba(6,182,212,0.2)`, display: "flex", alignItems: "center", gap: 5 }}>
            <Icon name="gateway" size={12} />
            <span style={{ fontSize: 10, fontWeight: 600, color: COLORS.accent, letterSpacing: 0.5, textTransform: "uppercase" }}>via YARP Gateway</span>
          </div>
        </div>
      </nav>

      <main style={{ maxWidth: 1100, margin: "0 auto", padding: "32px 24px" }}>

        {page === "manage" && (
          <div style={{ animation: "fadeUp 0.4s ease" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 28 }}>
              <div>
                <h1 style={{ fontSize: 28, fontWeight: 700, margin: 0 }}>Arrang\u00f8r-panel</h1>
                <p style={{ color: COLORS.textMuted, fontSize: 14, margin: "6px 0 0" }}>Opret og administr\u00e9r dine events (US-01, US-02, US-03)</p>
              </div>
              <Btn onClick={() => navigateTo("createEvent")} icon="plus">Opret nyt event</Btn>
            </div>
            <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
              {events.map((ev, i) => (
                <div key={ev.id} onClick={() => navigateTo("manageEvent", ev)}
                  style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: "16px 20px", cursor: "pointer", transition: "all 0.2s", display: "flex", alignItems: "center", gap: 16, animation: `fadeUp 0.4s ease ${i * 0.05}s both` }}
                  onMouseEnter={e => { e.currentTarget.style.background = COLORS.cardHover; }} onMouseLeave={e => { e.currentTarget.style.background = COLORS.card; }}>
                  <div style={{ flex: 1 }}>
                    <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 4 }}>
                      <span style={{ fontWeight: 600, fontSize: 15 }}>{ev.name}</span>
                      <StatusBadge status={ev.status} />
                    </div>
                    <div style={{ display: "flex", gap: 16, fontSize: 12, color: COLORS.textDim }}>
                      <span style={{ display: "flex", alignItems: "center", gap: 4 }}><Icon name="calendar" size={12} />{new Date(ev.date).toLocaleDateString("da-DK", { day: "numeric", month: "short", year: "numeric" })}</span>
                      <span style={{ display: "flex", alignItems: "center", gap: 4 }}><Icon name="pin" size={12} />{ev.location}</span>
                      <span>{ev.categories.length} kat.</span>
                    </div>
                  </div>
                  <Icon name="chevron" size={16} />
                </div>
              ))}
            </div>
          </div>
        )}

        {page === "createEvent" && (
          <div style={{ animation: "fadeUp 0.4s ease", maxWidth: 560, margin: "0 auto" }}>
            <BackBtn onClick={() => navigateTo("manage")} label="Tilbage til arrang\u00f8r-panel" />
            <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 28 }}>
              <h2 style={{ fontSize: 20, fontWeight: 700, margin: "0 0 4px" }}>Opret nyt event</h2>
              <p style={{ color: COLORS.textDim, fontSize: 12, margin: "0 0 24px" }}>US-01 \u00b7 Eventet oprettes med status "Draft" og tildeles et unikt ID</p>
              <InputField label="Eventnavn" value={eventForm.name} onChange={v => setEventForm(f => ({ ...f, name: v }))} placeholder="f.eks. Copenhagen Jazz Festival" error={eventErrors.name} />
              <InputField label="Dato" type="date" value={eventForm.date} onChange={v => setEventForm(f => ({ ...f, date: v }))} error={eventErrors.date} />
              <InputField label="Sted" value={eventForm.location} onChange={v => setEventForm(f => ({ ...f, location: v }))} placeholder="f.eks. Tivoli Gardens, K\u00f8benhavn" error={eventErrors.location} />
              <InputField label="Beskrivelse" value={eventForm.description} onChange={v => setEventForm(f => ({ ...f, description: v }))} placeholder="Beskriv eventet..." textarea />
              <Btn onClick={handleCreateEvent} icon="plus" full>Opret event</Btn>
              <ApiFlowBox label="US-01 \u00b7 Command" text="Blazor \u2192 YARP Gateway \u2192 Event Catalog (POST /events) \u2192 Event oprettet med status Draft" />
            </div>
          </div>
        )}

        {page === "manageEvent" && editingEvent && (() => {
          const ev = events.find(e => e.id === editingEvent.id) || editingEvent;
          return (
            <div style={{ animation: "fadeUp 0.4s ease", maxWidth: 700, margin: "0 auto" }}>
              <BackBtn onClick={() => navigateTo("manage")} label="Tilbage til arrang\u00f8r-panel" />
              <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 24, marginBottom: 16 }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                  <div>
                    <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 6 }}>
                      <h2 style={{ fontSize: 22, fontWeight: 700, margin: 0 }}>{ev.name}</h2>
                      <StatusBadge status={ev.status} />
                    </div>
                    <div style={{ display: "flex", gap: 16, fontSize: 13, color: COLORS.textDim }}>
                      <span style={{ display: "flex", alignItems: "center", gap: 5 }}><Icon name="calendar" size={13} />{new Date(ev.date).toLocaleDateString("da-DK", { weekday: "long", day: "numeric", month: "long", year: "numeric" })}</span>
                      <span style={{ display: "flex", alignItems: "center", gap: 5 }}><Icon name="pin" size={13} />{ev.location}</span>
                    </div>
                    {ev.description && <p style={{ fontSize: 13, color: COLORS.textMuted, margin: "8px 0 0", lineHeight: 1.6 }}>{ev.description}</p>}
                  </div>
                  {ev.status === "Draft" && <Btn onClick={handlePublishEvent} variant="success" icon="send">Publ\u00edc\u00e9r</Btn>}
                </div>
                {ev.status === "Draft" && <ApiFlowBox label="US-03 \u00b7 Publicer" text="Publicer \u2192 YARP \u2192 Event Catalog (PUT /events/{id}/publish) \u2192 status Published \u2192 EventCreated pub/sub \u2192 Inventory opretter TicketStock" />}
                {ev.status === "Published" && <ApiFlowBox label="\u2713 Publiceret" text="EventCreated sendt via Dapr pub/sub \u2192 Inventory har oprettet lagerposter for alle kategorier" />}
              </div>
              <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 24 }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
                  <div>
                    <h3 style={{ fontSize: 16, fontWeight: 600, margin: 0 }}>Billetkategorier</h3>
                    <p style={{ fontSize: 12, color: COLORS.textDim, margin: "2px 0 0" }}>US-02 \u00b7 Tilf\u00f8j kategorier med navn, pris og antal</p>
                  </div>
                  <span style={{ fontSize: 12, color: COLORS.textDim }}>{ev.categories.length} kat.</span>
                </div>
                {ev.categories.length > 0 && <div style={{ display: "flex", flexDirection: "column", gap: 8, marginBottom: 20 }}>
                  {ev.categories.map(c => (
                    <div key={c.id} style={{ display: "flex", alignItems: "center", padding: "12px 16px", background: COLORS.surface, borderRadius: 10, border: `1px solid ${COLORS.border}` }}>
                      <div style={{ flex: 1 }}>
                        <span style={{ fontWeight: 600, fontSize: 14 }}>{c.name}</span>
                        <div style={{ fontSize: 12, color: COLORS.textDim, marginTop: 2 }}>
                          <span style={{ fontFamily: "'Space Mono', monospace", color: COLORS.primary, fontWeight: 700 }}>{c.price} kr</span>
                          <span style={{ margin: "0 8px", color: COLORS.border }}>\u00b7</span>{c.total} billetter
                        </div>
                      </div>
                      {ev.status === "Draft" && <button onClick={() => handleRemoveCategory(c.id)} style={{ width: 32, height: 32, borderRadius: 8, border: `1px solid rgba(239,68,68,0.2)`, background: "rgba(239,68,68,0.08)", color: COLORS.danger, display: "flex", alignItems: "center", justifyContent: "center", cursor: "pointer" }}><Icon name="trash" size={14} /></button>}
                    </div>
                  ))}
                </div>}
                {ev.status === "Draft" && (
                  <div style={{ padding: 16, background: COLORS.surface, borderRadius: 10, border: `1px dashed ${COLORS.borderLight}` }}>
                    <div style={{ fontSize: 13, fontWeight: 600, marginBottom: 12, color: COLORS.textMuted }}>Tilf\u00f8j kategori</div>
                    <div style={{ display: "grid", gridTemplateColumns: "1fr 100px 100px auto", gap: 10, alignItems: "end" }}>
                      <InputField label="Navn" value={newCat.name} onChange={v => setNewCat(c => ({ ...c, name: v }))} placeholder="f.eks. VIP" error={catErrors.name} />
                      <InputField label="Pris (kr)" type="number" value={newCat.price} onChange={v => setNewCat(c => ({ ...c, price: v }))} placeholder="0" error={catErrors.price} />
                      <InputField label="Antal" type="number" value={newCat.total} onChange={v => setNewCat(c => ({ ...c, total: v }))} placeholder="0" error={catErrors.total} />
                      <div style={{ marginBottom: 16 }}><Btn onClick={handleAddCategory} icon="plus" small>Tilf\u00f8j</Btn></div>
                    </div>
                  </div>
                )}
                {ev.status === "Published" && <div style={{ padding: 12, background: "rgba(249,115,22,0.06)", borderRadius: 8, border: `1px solid ${COLORS.tagBorder}`, fontSize: 12, color: COLORS.textMuted }}>Kategorier kan ikke \u00e6ndres efter publicering \u2014 Inventory har allerede oprettet lagerposter.</div>}
              </div>
            </div>
          );
        })()}

        {page === "events" && (
          <div>
            <div style={{ marginBottom: 32 }}>
              <h1 style={{ fontSize: 28, fontWeight: 700, margin: 0 }}>Kommende Events</h1>
              <p style={{ color: COLORS.textMuted, fontSize: 14, margin: "6px 0 0" }}>Browse og find billetter til de bedste events</p>
            </div>
            <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(320px, 1fr))", gap: 16 }}>
              {publishedEvents.map((ev, i) => (
                <div key={ev.id} onClick={() => navigateTo("eventDetail", ev)} style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 20, cursor: "pointer", transition: "all 0.25s ease", position: "relative", overflow: "hidden", animation: `fadeUp 0.5s ease ${i * 0.08}s both` }}
                  onMouseEnter={e => { e.currentTarget.style.background = COLORS.cardHover; e.currentTarget.style.transform = "translateY(-2px)"; }}
                  onMouseLeave={e => { e.currentTarget.style.background = COLORS.card; e.currentTarget.style.transform = "translateY(0)"; }}>
                  <div style={{ position: "absolute", top: 0, left: 0, right: 0, height: 3, background: `linear-gradient(90deg, ${COLORS.primary}, ${COLORS.accent})`, opacity: 0.6 }} />
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 12 }}>
                    <h3 style={{ fontSize: 17, fontWeight: 600, margin: 0, lineHeight: 1.3, flex: 1 }}>{ev.name}</h3>
                    <StatusBadge status={ev.status} />
                  </div>
                  <div style={{ display: "flex", flexDirection: "column", gap: 6, marginBottom: 14 }}>
                    <div style={{ display: "flex", alignItems: "center", gap: 6, color: COLORS.textMuted, fontSize: 13 }}><Icon name="calendar" size={13} />{new Date(ev.date).toLocaleDateString("da-DK", { day: "numeric", month: "long", year: "numeric" })}</div>
                    <div style={{ display: "flex", alignItems: "center", gap: 6, color: COLORS.textMuted, fontSize: 13 }}><Icon name="pin" size={13} />{ev.location}</div>
                  </div>
                  <div style={{ display: "flex", gap: 6, flexWrap: "wrap" }}>
                    {ev.categories.map(c => <span key={c.id} style={{ padding: "3px 8px", borderRadius: 6, fontSize: 11, fontWeight: 500, background: COLORS.tagBg, color: COLORS.primary, border: `1px solid ${COLORS.tagBorder}` }}>{c.name} \u00b7 {c.price} kr</span>)}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {page === "eventDetail" && selectedEvent && (
          <div style={{ animation: "fadeUp 0.4s ease" }}>
            <BackBtn onClick={() => navigateTo("events")} label="Tilbage til events" />
            <div style={{ display: "grid", gridTemplateColumns: "1fr 380px", gap: 24 }}>
              <div>
                <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 8 }}>
                  <h1 style={{ fontSize: 26, fontWeight: 700, margin: 0 }}>{selectedEvent.name}</h1>
                  <StatusBadge status={selectedEvent.status} />
                </div>
                <div style={{ display: "flex", gap: 20, marginBottom: 20 }}>
                  <span style={{ display: "flex", alignItems: "center", gap: 6, color: COLORS.textMuted, fontSize: 13 }}><Icon name="calendar" size={14} />{new Date(selectedEvent.date).toLocaleDateString("da-DK", { weekday: "long", day: "numeric", month: "long", year: "numeric" })}</span>
                  <span style={{ display: "flex", alignItems: "center", gap: 6, color: COLORS.textMuted, fontSize: 13 }}><Icon name="pin" size={14} />{selectedEvent.location}</span>
                </div>
                <p style={{ color: COLORS.textMuted, fontSize: 14, lineHeight: 1.7, margin: "0 0 28px", maxWidth: 560 }}>{selectedEvent.description}</p>
                <h3 style={{ fontSize: 14, fontWeight: 600, margin: "0 0 12px", textTransform: "uppercase", letterSpacing: 1, color: COLORS.textDim }}>Billetkategorier</h3>
                <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
                  {selectedEvent.categories.map(c => (
                    <div key={c.id} style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 10, padding: 16 }}>
                      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 8 }}>
                        <span style={{ fontWeight: 600, fontSize: 15 }}>{c.name}</span>
                        <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 17, color: COLORS.primary }}>{c.price} kr</span>
                      </div>
                      <AvailabilityBar available={c.available} total={c.total} />
                      <div style={{ fontSize: 11, color: COLORS.textDim, marginTop: 4 }}>{c.available === 0 ? "Udsolgt" : `${c.available} billetter tilg\u00e6ngelige`}</div>
                    </div>
                  ))}
                </div>
              </div>
              <div style={{ position: "sticky", top: 80, alignSelf: "start" }}>
                <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 24 }}>
                  <h3 style={{ fontSize: 16, fontWeight: 600, margin: "0 0 4px" }}>K\u00f8b billetter</h3>
                  <p style={{ color: COLORS.textDim, fontSize: 12, margin: "0 0 20px" }}>V\u00e6lg kategori og antal for at bestille</p>
                  <Btn onClick={() => navigateTo("order", selectedEvent)} icon="cart" full>G\u00e5 til bestilling</Btn>
                  <ApiFlowBox text="Blazor UI \u2192 YARP Gateway \u2192 Event Catalog + Inventory" />
                </div>
              </div>
            </div>
          </div>
        )}

        {page === "order" && orderCart && (
          <div style={{ animation: "fadeUp 0.4s ease", maxWidth: 600, margin: "0 auto" }}>
            <BackBtn onClick={() => navigateTo("eventDetail", orderCart)} label={`Tilbage til ${orderCart.name}`} />
            {!orderResult ? (
              <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 28 }}>
                <h2 style={{ fontSize: 20, fontWeight: 700, margin: "0 0 4px" }}>Bestil billetter</h2>
                <p style={{ color: COLORS.textMuted, fontSize: 13, margin: "0 0 24px" }}>{orderCart.name}</p>
                {orderCart.categories.map(c => (
                  <div key={c.id} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "14px 0", borderBottom: `1px solid ${COLORS.border}` }}>
                    <div><div style={{ fontWeight: 600, fontSize: 14 }}>{c.name}</div><div style={{ fontSize: 12, color: COLORS.textDim }}>{c.price} kr \u00b7 {c.available} tilg.</div></div>
                    <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                      <button disabled={!quantities[c.id]} onClick={() => setQuantities(q => ({ ...q, [c.id]: Math.max(0, (q[c.id] || 0) - 1) }))} style={{ width: 32, height: 32, borderRadius: 8, border: `1px solid ${COLORS.border}`, background: COLORS.surface, color: COLORS.text, fontSize: 16, cursor: "pointer", display: "flex", alignItems: "center", justifyContent: "center", fontFamily: "inherit" }}>{"\u2212"}</button>
                      <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 16, width: 24, textAlign: "center" }}>{quantities[c.id] || 0}</span>
                      <button disabled={c.available === 0 || (quantities[c.id] || 0) >= c.available} onClick={() => setQuantities(q => ({ ...q, [c.id]: Math.min(c.available, (q[c.id] || 0) + 1) }))} style={{ width: 32, height: 32, borderRadius: 8, border: `1px solid ${COLORS.border}`, background: COLORS.surface, color: COLORS.text, fontSize: 16, cursor: "pointer", display: "flex", alignItems: "center", justifyContent: "center", fontFamily: "inherit" }}>+</button>
                    </div>
                  </div>
                ))}
                {(() => { const total = Object.entries(quantities).reduce((s, [id, q]) => s + q * (orderCart.categories.find(c => c.id === parseInt(id))?.price || 0), 0); return (
                  <div style={{ marginTop: 20 }}>
                    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
                      <span style={{ fontSize: 14, fontWeight: 600 }}>Total</span>
                      <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 22, color: COLORS.primary }}>{total.toLocaleString("da-DK")} kr</span>
                    </div>
                    <Btn disabled={total === 0} onClick={handleOrder} full>Plac\u00e9r bestilling</Btn>
                  </div>); })()}
                <ApiFlowBox label="SAGA-flow" text="Blazor \u2192 YARP \u2192 Ordering \u2192 ReserveTickets \u2192 ProcessPayment \u2192 ConfirmOrder" />
              </div>
            ) : (
              <div style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 14, padding: 28, textAlign: "center" }}>
                <div style={{ width: 48, height: 48, borderRadius: "50%", background: "rgba(34,197,94,0.15)", display: "flex", alignItems: "center", justifyContent: "center", margin: "0 auto 16px", color: COLORS.success }}><Icon name="check" size={24} /></div>
                <h2 style={{ fontSize: 20, fontWeight: 700, margin: "0 0 4px" }}>Bestilling modtaget!</h2>
                <p style={{ color: COLORS.textMuted, fontSize: 13, margin: "0 0 20px" }}>SAGA-workflowet behandler din bestilling</p>
                <div style={{ background: COLORS.surface, borderRadius: 10, padding: 16, textAlign: "left", marginBottom: 20 }}>
                  <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8 }}><span style={{ color: COLORS.textDim, fontSize: 12 }}>Ordre-ID</span><span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 13 }}>{orderResult.id}</span></div>
                  <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8 }}><span style={{ color: COLORS.textDim, fontSize: 12 }}>Status</span><StatusBadge status={orderResult.status} /></div>
                  <div style={{ display: "flex", justifyContent: "space-between" }}><span style={{ color: COLORS.textDim, fontSize: 12 }}>Total</span><span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, color: COLORS.primary }}>{orderResult.total.toLocaleString("da-DK")} kr</span></div>
                </div>
                <Btn onClick={() => navigateTo("orders")} variant="secondary">Se mine ordrer</Btn>
              </div>
            )}
          </div>
        )}

        {page === "orders" && (
          <div style={{ animation: "fadeUp 0.4s ease" }}>
            <h1 style={{ fontSize: 28, fontWeight: 700, margin: "0 0 6px" }}>Mine Ordrer</h1>
            <p style={{ color: COLORS.textMuted, fontSize: 14, margin: "0 0 28px" }}>Oversigt over dine bestillinger</p>
            <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
              {initialOrders.map((o, i) => (
                <div key={o.id} style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 20, animation: `fadeUp 0.4s ease ${i * 0.06}s both` }}>
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 12 }}>
                    <div>
                      <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 4 }}><span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 14 }}>{o.id}</span><StatusBadge status={o.status} /></div>
                      <div style={{ fontSize: 15, fontWeight: 600, marginBottom: 2 }}>{o.eventName}</div>
                      <div style={{ fontSize: 12, color: COLORS.textDim }}>{new Date(o.createdAt).toLocaleDateString("da-DK", { day: "numeric", month: "long", year: "numeric", hour: "2-digit", minute: "2-digit" })}</div>
                    </div>
                    <span style={{ fontFamily: "'Space Mono', monospace", fontWeight: 700, fontSize: 18, color: COLORS.primary }}>{o.total.toLocaleString("da-DK")} kr</span>
                  </div>
                  <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
                    {o.lines.map((l, j) => <span key={j} style={{ padding: "3px 8px", borderRadius: 6, fontSize: 11, background: COLORS.tagBg, color: COLORS.primary, border: `1px solid ${COLORS.tagBorder}` }}>{l.qty}\u00d7 {l.category} ({l.unitPrice} kr)</span>)}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {page === "health" && (
          <div style={{ animation: "fadeUp 0.4s ease" }}>
            <h1 style={{ fontSize: 28, fontWeight: 700, margin: "0 0 6px" }}>Systemsundhed</h1>
            <p style={{ color: COLORS.textMuted, fontSize: 14, margin: "0 0 28px" }}>Status fra YARP API Gateway /health endpoint</p>
            <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))", gap: 12 }}>
              {[
                { name: "API Gateway", port: 5000, tech: "YARP", status: "healthy" },
                { name: "Event Catalog", port: 5001, tech: "Dapr Pub/Sub", status: "healthy" },
                { name: "Inventory", port: 5002, tech: "Dapr Sub + SI", status: "healthy" },
                { name: "Ordering", port: 5003, tech: "Dapr Workflow", status: "healthy" },
                { name: "Payment", port: 5004, tech: "Stateless", status: "degraded" },
                { name: "Notification", port: 5005, tech: "Dapr Sub", status: "healthy" },
                { name: "Blazor UI", port: 5010, tech: "Blazor Server", status: "healthy" },
              ].map((s, i) => (
                <div key={s.name} style={{ background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 12, padding: 18, animation: `fadeUp 0.4s ease ${i * 0.06}s both` }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 10 }}>
                    <div style={{ width: 8, height: 8, borderRadius: "50%", background: s.status === "healthy" ? COLORS.success : COLORS.warning, boxShadow: s.status === "healthy" ? `0 0 8px ${COLORS.success}` : `0 0 8px ${COLORS.warning}` }} />
                    <span style={{ fontWeight: 600, fontSize: 14 }}>{s.name}</span>
                  </div>
                  <div style={{ fontSize: 11, color: COLORS.textDim, marginBottom: 4 }}>:{s.port} \u00b7 {s.tech}</div>
                  <span style={{ fontSize: 10, fontWeight: 600, textTransform: "uppercase", letterSpacing: 0.5, color: s.status === "healthy" ? COLORS.success : COLORS.warning }}>{s.status}</span>
                </div>
              ))}
            </div>
            <div style={{ marginTop: 24, padding: 16, background: COLORS.card, border: `1px solid ${COLORS.border}`, borderRadius: 12 }}>
              <div style={{ fontFamily: "'Space Mono', monospace", fontSize: 12, color: COLORS.textDim }}>
                <span style={{ color: COLORS.accent }}>GET</span> /health \u2192 200 OK \u00b7 Aggregeret fra alle services via YARP reverse proxy
              </div>
            </div>
          </div>
        )}
      </main>
      <style>{`
        @keyframes fadeUp { from { opacity: 0; transform: translateY(12px); } to { opacity: 1; transform: translateY(0); } }
        * { box-sizing: border-box; }
        button:focus { outline: 2px solid ${COLORS.primary}; outline-offset: 2px; }
      `}</style>
    </div>
  );
}
