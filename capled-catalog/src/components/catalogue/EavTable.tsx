import React from 'react';

type EavSpec = {
  nom: string;
  valeur: string;
  unite?: string;
};

export const EavTable = ({ specs }: { specs: EavSpec[] }) => {
  if (!specs || specs.length === 0) {
    return (
      <div className="alert alert-secondary mb-0">
        Aucune spécification technique renseignée.
      </div>
    );
  }

  return (
    <div className="table-responsive rounded border-0">
      <table className="table table-striped table-hover mb-0 eav-table border-0">
        <tbody>
          {specs.map((spec, idx) => (
            <tr key={idx}>
              <th scope="row" className="text-dark fw-bold align-middle py-3 px-4 border-0">
                {spec.nom}
              </th>
              <td className="align-middle text-secondary py-3 px-4 border-0" style={{ whiteSpace: 'pre-wrap' }}>
                {spec.valeur} {spec.unite || ''}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
