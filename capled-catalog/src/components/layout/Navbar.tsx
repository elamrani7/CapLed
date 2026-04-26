import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { catalogueApi } from '../../api/catalogueApi';

export const Navbar = () => {
  const { totalItems } = useCart();
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedFamille, setSelectedFamille] = useState('');
  const [familles, setFamilles] = useState<any[]>([]);
  const [showFamillesDropdown, setShowFamillesDropdown] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    let mounted = true;
    catalogueApi.getFamilles()
      .then(f => { if (mounted) setFamilles(Array.isArray(f) ? f : []); })
      .catch(() => {});
    return () => { mounted = false; };
  }, []);

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 10);
    window.addEventListener('scroll', onScroll);
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  // Close mobile menu on route change
  useEffect(() => { setMobileOpen(false); }, [location.pathname]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const params = new URLSearchParams();
    if (searchQuery.trim()) params.append('search', searchQuery.trim());
    if (selectedFamille) params.append('familleId', selectedFamille);
    navigate(`/catalogue?${params.toString()}`);
    setSearchQuery('');
  };

  const isActive = (path: string) =>
    location.pathname === path ? 'pf-nav-link active' : 'pf-nav-link';

  return (
    <header className="pf-navbar" style={{ boxShadow: scrolled ? '0 2px 12px rgba(0,0,0,0.1)' : '0 1px 3px rgba(0,0,0,0.05)' }}>
      <div className="container-xl">
        {/* ── Main Row ── */}
        <div className="pf-nav-inner">
          {/* Logo */}
          <Link to="/" className="text-decoration-none d-flex align-items-center gap-2 flex-shrink-0">
            <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0"
              style={{ width: 34, height: 34, fontSize: '1.1rem' }}>
              <i className="bi bi-gears"></i>
            </div>
            <span>
              <span className="pf-logo-part">Part</span>
              <span className="pf-logo-name"> Finder</span>
              <span className="pf-logo-part" style={{ fontSize: '1rem' }}> .ma</span>
            </span>
          </Link>

          {/* Desktop Nav */}
          <nav className="d-none d-lg-flex">
            <ul className="pf-nav-links">
              <li><Link to="/" className={isActive('/')}>Accueil</Link></li>

              {/* Familles dropdown */}
              <li className="pf-nav-dropdown"
                onMouseEnter={() => setShowFamillesDropdown(true)}
                onMouseLeave={() => setShowFamillesDropdown(false)}>
                <span className="pf-nav-link" style={{ cursor: 'pointer', userSelect: 'none' }}>
                  Familles&nbsp;<i className="bi bi-chevron-down" style={{ fontSize: '0.65rem' }}></i>
                </span>
                {showFamillesDropdown && familles.length > 0 && (
                  <div className="pf-nav-dropdown-menu">
                    {familles.map(f => (
                      <Link key={f.id} to={`/catalogue?familleId=${f.id}`}
                        className="pf-nav-dropdown-item"
                        onClick={() => setShowFamillesDropdown(false)}>
                        <i className="bi bi-tools" style={{ color: 'var(--pf-navy)', opacity: 0.5 }}></i>
                        {f.libelle || f.code}
                      </Link>
                    ))}
                  </div>
                )}
              </li>

              <li><Link to="/catalogue" className={isActive('/catalogue')}>Catalogue</Link></li>
              <li><Link to="/contact" className={isActive('/contact')}>Contact</Link></li>
              <li>
                <Link to={isAuthenticated ? '/cart' : '/login'} className={isActive('/login')}>
                  {isAuthenticated ? 'Mon Espace' : 'Connexion'}
                </Link>
              </li>
            </ul>
          </nav>

          {/* Right actions */}
          <div className="d-flex align-items-center gap-2">
            <Link to="/cart" className="pf-cart-btn text-decoration-none">
              <i className="bi bi-cart3" style={{ fontSize: '1.2rem' }}></i>
              {totalItems > 0 && <span className="pf-cart-badge">{totalItems}</span>}
              <span className="d-none d-xl-inline" style={{ color: 'var(--pf-gray-800)' }}>Devis</span>
            </Link>

            {isAuthenticated ? (
              <div className="dropdown">
                <div className="d-flex align-items-center gap-2 px-2 py-1 rounded" role="button"
                  data-bs-toggle="dropdown" style={{ cursor: 'pointer' }}>
                  <i className="bi bi-person-check-fill" style={{ color: 'var(--pf-green)', fontSize: '1.15rem' }}></i>
                  <span className="d-none d-lg-inline fw-semibold" style={{ fontSize: 'var(--text-sm)' }}>
                    {user?.fullName?.split(' ')[0]}
                  </span>
                  <i className="bi bi-chevron-down" style={{ fontSize: '0.65rem', opacity: 0.5 }}></i>
                </div>
                <ul className="dropdown-menu dropdown-menu-end shadow border-0" style={{ borderRadius: '10px', marginTop: '6px' }}>
                  <li><span className="dropdown-item-text" style={{ fontSize: 'var(--text-xs)', color: 'var(--pf-gray-400)' }}>
                    {user?.email}
                  </span></li>
                  <li><hr className="dropdown-divider my-1" /></li>
                  <li>
                    <button className="dropdown-item text-danger" style={{ fontSize: 'var(--text-sm)' }}
                      onClick={() => { logout(); navigate('/'); }}>
                      <i className="bi bi-box-arrow-left me-2"></i>Se déconnecter
                    </button>
                  </li>
                </ul>
              </div>
            ) : (
              <Link to="/contact" className="pf-btn-contact d-none d-md-inline-flex">
                <i className="bi bi-telephone-fill"></i>
                Nous contacter
              </Link>
            )}

            {/* Mobile toggle */}
            <button className="d-lg-none btn btn-sm border-0 p-1" onClick={() => setMobileOpen(o => !o)}>
              <i className={`bi ${mobileOpen ? 'bi-x-lg' : 'bi-list'} fs-4`}></i>
            </button>
          </div>
        </div>

        {/* Mobile Menu */}
        {mobileOpen && (
          <div className="border-top py-3 d-lg-none">
            <form onSubmit={handleSearch} className="mb-3">
              <div className="input-group input-group-sm">
                <input type="text" className="form-control" placeholder="Rechercher..."
                  value={searchQuery} onChange={e => setSearchQuery(e.target.value)} />
                <button type="submit" className="btn btn-primary"><i className="bi bi-search"></i></button>
              </div>
            </form>
            <nav className="d-flex flex-column gap-1">
              {[
                { to: '/', label: 'Accueil' },
                { to: '/catalogue', label: 'Catalogue' },
                { to: '/contact', label: 'Contact' },
                { to: '/login', label: 'Connexion' },
              ].map(({ to, label }) => (
                <Link key={to} to={to} className="pf-nav-link">{label}</Link>
              ))}
              {familles.length > 0 && (
                <>
                  <div className="pf-nav-link fw-semibold" style={{ color: 'var(--pf-gray-400)', fontSize: '0.75rem', textTransform: 'uppercase', letterSpacing: '0.5px' }}>
                    Familles
                  </div>
                  {familles.map(f => (
                    <Link key={f.id} to={`/catalogue?familleId=${f.id}`}
                      className="pf-nav-link ps-4" style={{ fontSize: 'var(--text-sm)' }}>
                      <i className="bi bi-tools me-2" style={{ opacity: 0.4 }}></i>{f.libelle || f.code}
                    </Link>
                  ))}
                </>
              )}
            </nav>
          </div>
        )}
      </div>

      {/* Search row — slim, always visible */}
      <div className="pf-searchbar-row">
        <div className="container-xl">
          <form onSubmit={handleSearch}>
            <div className="pf-hero-search" style={{ maxWidth: '860px', margin: '0 auto' }}>
              <select className="pf-hero-search-select d-none d-md-block"
                value={selectedFamille} onChange={e => setSelectedFamille(e.target.value)}>
                <option value="">Toutes les familles</option>
                {familles.map(f => <option key={f.id} value={f.id}>{f.libelle || f.code}</option>)}
              </select>
              <input type="text" className="pf-hero-search-input"
                placeholder="Référence, désignation, marque..."
                value={searchQuery} onChange={e => setSearchQuery(e.target.value)} />
              <button type="submit" className="pf-hero-search-btn">
                <i className="bi bi-search me-1"></i>
                <span className="d-none d-sm-inline">Rechercher</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </header>
  );
};
