import React, { useState, useEffect } from 'react';
import { useSearchParams, useLocation } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import { catalogueApi } from '../api/catalogueApi';
import { ProductCard } from '../components/catalogue/ProductCard';
import { Loader } from '../components/shared/Loader';
import { Pagination } from '../components/shared/Pagination';

export const CataloguePage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const location = useLocation();
  
  // Controls state
  const [searchText, setSearchText] = useState(searchParams.get('search') || '');
  const [selectedFamille, setSelectedFamille] = useState(searchParams.get('familleId') || '');
  const [selectedCategorie, setSelectedCategorie] = useState(searchParams.get('categorieId') || '');
  const [selectedCondition, setSelectedCondition] = useState(searchParams.get('condition') || '');
  const [dynamicSpecs, setDynamicSpecs] = useState<Record<string, string>>({});

  // Meta data state
  const [familles, setFamilles] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);

  // Results state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [products, setProducts] = useState<any[]>([]);
  const [page, setPage] = useState(parseInt(searchParams.get('page') || '1', 10));
  const [totalItems, setTotalItems] = useState(0);
  const pageSize = 12;

  // Synchronize external URL changes (e.g. from Navbar) with local state
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const urlSearch = params.get('search') || '';
    const urlFamille = params.get('familleId') || '';
    const urlCategorie = params.get('categorieId') || '';
    
    if (urlSearch !== searchText) setSearchText(urlSearch);
    if (urlFamille !== selectedFamille) setSelectedFamille(urlFamille);
    if (urlCategorie !== selectedCategorie) setSelectedCategorie(urlCategorie);
    
    // Automatically reset page to 1 when a new search comes from the header
    if (urlSearch !== searchText || urlFamille !== selectedFamille || urlCategorie !== selectedCategorie) {
      setPage(1);
    }
  }, [location.search]);

  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        const famRes = await catalogueApi.getFamilles();
        setFamilles(Array.isArray(famRes) ? famRes : []);
      } catch (err) {
        console.warn('Could not load familles', err);
      }
    };
    fetchMetadata();
  }, []);

  useEffect(() => {
    const fetchCategories = async () => {
      if (!selectedFamille) {
        setCategories([]);
        return;
      }
      try {
        const catRes = await catalogueApi.getCategories(parseInt(selectedFamille, 10));
        setCategories(Array.isArray(catRes) ? catRes : []);
      } catch (err) {
        console.warn('Could not load categories', err);
      }
    };
    fetchCategories();
  }, [selectedFamille]);

  useEffect(() => {
    const fetchCatalogue = async () => {
      setLoading(true);
      setError('');
      try {
        const filters: any = {
          page,
          pageSize,
          search: searchText,
          familleId: selectedFamille,
          categorieId: selectedCategorie,
          condition: selectedCondition,
          ...dynamicSpecs
        };

        const res: any = await catalogueApi.getCatalogue(filters);
        const items = res.items || [];
        const total = res.totalCount || items.length;

        setProducts(items);
        setTotalItems(total);

        const paramsToUpdate: any = { page: page.toString() };
        if (searchText) paramsToUpdate.search = searchText;
        if (selectedFamille) paramsToUpdate.familleId = selectedFamille;
        if (selectedCategorie) paramsToUpdate.categorieId = selectedCategorie;
        if (selectedCondition) paramsToUpdate.condition = selectedCondition;
        
        setSearchParams(paramsToUpdate, { replace: true });
        
      } catch (err: any) {
        setError(err.message || 'Erreur lors du chargement des articles.');
      } finally {
        setLoading(false);
      }
    };

    fetchCatalogue();
  }, [page, searchText, selectedFamille, selectedCategorie, selectedCondition, dynamicSpecs]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
  };

  const handleFilterChange = () => {
    setPage(1);
  };

  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />
      <main className="flex-grow-1 container-fluid px-4 py-4">
        <div className="d-flex justify-content-between align-items-end mb-3 border-bottom pb-2">
          <h1 className="fw-bolder text-dark m-0 fs-3">Catalogue Public</h1>
          <span className="text-secondary small fw-bold">{totalItems} article(s) trouvé(s)</span>
        </div>

        <div className="row g-3">
          
          {/* Sidebar Filtrage (Auto-Doc Style) */}
          <aside className="col-lg-3">
            <div className="card shadow-sm border-0 mb-4">
              <div className="sidebar-filter-header d-flex justify-content-between align-items-center">
                <span><i className="bi bi-funnel-fill me-2 text-primary"></i>Filtres</span>
                <button 
                  type="button"
                  className="btn btn-sm btn-link text-decoration-none text-muted p-0"
                  onClick={() => {
                    setSearchText('');
                    setSelectedFamille('');
                    setSelectedCategorie('');
                    setSelectedCondition('');
                    setPage(1);
                  }}
                >
                  <i className="bi bi-arrow-counterclockwise"></i>
                </button>
              </div>
              
              <div className="card-body p-3">
                <form onSubmit={handleSearchSubmit} className="d-flex flex-column gap-3">
                  
                  {/* Recherche Locale */}
                  <div className="mb-2">
                    <label className="form-label small fw-bold text-dark">Mots-clés</label>
                    <div className="input-group input-group-sm">
                      <input 
                        type="text" 
                        placeholder="Ref, nom..." 
                        value={searchText}
                        onChange={(e) => setSearchText(e.target.value)}
                        className="form-control"
                      />
                      <button type="submit" className="btn btn-secondary">
                        <i className="bi bi-search"></i>
                      </button>
                    </div>
                  </div>
                  
                  <hr className="my-1 text-muted" />

                  {/* Famille */}
                  <div>
                    <label className="form-label small fw-bold text-dark">Famille</label>
                    <select 
                      value={selectedFamille}
                      onChange={(e) => {
                        setSelectedFamille(e.target.value);
                        setSelectedCategorie('');
                        handleFilterChange();
                      }}
                      className="form-select form-select-sm"
                    >
                      <option value="">Toutes les familles</option>
                      {familles.map((f: any) => (
                        <option key={f.id} value={f.id}>{f.libelle || f.code}</option>
                      ))}
                    </select>
                  </div>

                  {/* Catégorie */}
                  <div className={!selectedFamille ? 'opacity-50' : ''}>
                    <label className="form-label small fw-bold text-dark">Catégorie</label>
                    <select 
                      value={selectedCategorie}
                      onChange={(e) => {
                        setSelectedCategorie(e.target.value);
                        handleFilterChange();
                      }}
                      disabled={!selectedFamille}
                      className="form-select form-select-sm"
                    >
                      <option value="">Toutes les catégories</option>
                      {categories.map((c: any) => (
                        <option key={c.id} value={c.id}>{c.label}</option>
                      ))}
                    </select>
                  </div>
                  
                  <hr className="my-1 text-muted" />

                  {/* Condition */}
                  <div>
                    <label className="form-label small fw-bold text-dark">État du matériel</label>
                    <select 
                      value={selectedCondition}
                      onChange={(e) => {
                        setSelectedCondition(e.target.value);
                        handleFilterChange();
                      }}
                      className="form-select form-select-sm"
                    >
                      <option value="">Tous les états</option>
                      <option value="NEUF">Neuf (Jamais utilisé)</option>
                      <option value="OCCASION">Occasion (Fonctionnel)</option>
                      <option value="RECONDITIONNE">Reconditionné</option>
                      <option value="EN_REPARATION">En Réparation</option>
                      <option value="ENDOMMAGE">Pour pièces (Endommagé)</option>
                    </select>
                  </div>
                </form>
              </div>
            </div>
          </aside>

          {/* Grille Articles */}
          <section className="col-lg-9">
            {loading ? (
              <div className="card shadow-sm border-0 p-5 text-center">
                <Loader message="Recherche des articles en cours..." />
              </div>
            ) : error ? (
              <div className="alert alert-danger" role="alert">
                <h4 className="alert-heading">Erreur !</h4>
                <p>{error}</p>
              </div>
            ) : products.length === 0 ? (
              <div className="card shadow-sm border-0 p-5 text-center">
                <div className="display-1 text-muted mb-3"><i className="bi bi-search"></i></div>
                <h4 className="fw-bold">Aucun article trouvé</h4>
                <p className="text-secondary">Essayez de modifier vos filtres ou vos termes de recherche.</p>
                <button 
                  onClick={() => {
                    setSearchText('');
                    setSelectedFamille('');
                    setSelectedCategorie('');
                    setSelectedCondition('');
                    setPage(1);
                  }}
                  className="btn btn-link text-primary mt-3"
                >
                  Réinitialiser les filtres
                </button>
              </div>
            ) : (
              <>
                <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-3 mb-4">
                  {products.map((p) => (
                    <div className="col" key={p.id}>
                      <ProductCard product={p} />
                    </div>
                  ))}
                </div>
                
                <Pagination 
                  page={page} 
                  total={totalItems} 
                  pageSize={pageSize} 
                  onPageChange={setPage} 
                />
              </>
            )}
          </section>

        </div>
      </main>
      <Footer />
    </div>
  );
};
