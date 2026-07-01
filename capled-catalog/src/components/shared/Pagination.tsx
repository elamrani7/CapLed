import React from 'react';

type PaginationProps = {
  page: number;
  total: number;
  pageSize: number;
  onPageChange: (newPage: number) => void;
};

export const Pagination = ({ page, total, pageSize, onPageChange }: PaginationProps) => {
  const totalPages = Math.ceil(total / pageSize);

  if (totalPages <= 1) return null;

  return (
    <nav className="d-flex justify-content-between align-items-center mt-4 border-top pt-3">
      <div className="text-muted small d-none d-sm-block">
        Affichage de <span className="fw-bold">{(page - 1) * pageSize + 1}</span> à{' '}
        <span className="fw-bold">{Math.min(page * pageSize, total)}</span> sur{' '}
        <span className="fw-bold">{total}</span> résultats
      </div>
      <ul className="pagination mb-0">
        <li className={`page-item ${page <= 1 ? 'disabled' : ''}`}>
          <button className="page-link" onClick={() => onPageChange(page - 1)} aria-label="Précédent">
            <span aria-hidden="true">&laquo;</span>
          </button>
        </li>
        <li className="page-item disabled">
          <span className="page-link">Page {page} / {totalPages}</span>
        </li>
        <li className={`page-item ${page >= totalPages ? 'disabled' : ''}`}>
          <button className="page-link" onClick={() => onPageChange(page + 1)} aria-label="Suivant">
            <span aria-hidden="true">&raquo;</span>
          </button>
        </li>
      </ul>
    </nav>
  );
};
